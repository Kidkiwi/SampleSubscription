using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using DG.Tweening;

public class SubscriptionHandler : IAPHandler
{
    #region Variables
    public static SubscriptionHandler instance { get; set; }

    [SerializeField] TextMeshProUGUI _txtSubInfo = null;
    [SerializeField] TextMeshProUGUI _txtMessage = null;
    [SerializeField] GameObject _pnSub = null;
    [SerializeField] GameObject _pnContent = null;

    [Header("IAP Buttons")]
    [SerializeField] TextMeshProUGUI _txtMonthlyTitle = null;
    [SerializeField] TextMeshProUGUI _txtMonthlyPrice = null;
    [SerializeField] TextMeshProUGUI _txtYearlyTitle = null;
    [SerializeField] TextMeshProUGUI _txtYearlyPrice = null;

    List<SubscriptionInfo> _lstSubscriptionInfo = new List<SubscriptionInfo>();
    #endregion

    #region Functions
    private void Awake()
    {
        instance = this;
        _txtMessage.text = "";
    }

    public override void PurchaseSuccess(Product product)
    {
        base.PurchaseSuccess(product);

        switch(product.definition.id)
        {
            case GlobalVariables._monthlySubID:
                _txtMessage.text = "Monthly sub purchased!";
                break;
            case GlobalVariables._yearlySubID:
                _txtMessage.text = "Yearly sub purchased!";
                break;
        }

#if UNITY_EDITOR
        ActivateContent();
#endif

        GetInfo(product);

        _txtMessage.color = Color.green;
    }

    public override void PurchaseFailed(Product product, PurchaseFailureReason purchaseFailureReason)
    {
        base.PurchaseFailed(product, purchaseFailureReason);

        Debug.Log("Purchase failed: " + product.definition.id);

        switch (product.definition.id)
        {
            case GlobalVariables._monthlySubID:
                _txtMessage.text = "Monthly sub failed!\n" + purchaseFailureReason;
                break;
            case GlobalVariables._yearlySubID:
                _txtMessage.text = "Yearly sub failed!\n" + purchaseFailureReason;
                break;
        }

        _txtMessage.color = Color.red;

        // Just in case content is active.
        _pnSub.SetActive(true);
        _pnContent.SetActive(false);
    }

    public override void GetInfo(Product p)
    {
        if (!p.hasReceipt)
            return;

        #if !UNITY_EDITOR
        try
        {
            SubscriptionManager subscriptionManager = new SubscriptionManager(p, null);

            if (subscriptionManager.getSubscriptionInfo() != null)
            {
                _lstSubscriptionInfo.Insert(0, subscriptionManager.getSubscriptionInfo());

                _txtSubInfo.text += "<color=#f00>Purchase Date:</color> " + _lstSubscriptionInfo[0].getPurchaseDate() + "\n"
                    + "<color=#f00>Auto Renewing:</color> " + _lstSubscriptionInfo[0].isAutoRenewing() + "\n"
                    + "<color=#f00>Free Trial:</color> " + _lstSubscriptionInfo[0].isFreeTrial() + "\n"
                    + "<color=#f00>Is Subscriped:</color> " + _lstSubscriptionInfo[0].isSubscribed() + "\n"
                    + "-----------------------\n";
            }
            else
            {
                Debug.Log("Getsubinfo was null");
            }

            // We check if our subscription still has remaining time, this makes sure even if the user
            // cancels but still has time left, that they get the content.
            //if( _lstSubscriptionInfo.Count > 0 && _lstSubscriptionInfo[0].getRemainingTime().TotalSeconds > 10 )
            if (_lstSubscriptionInfo[0].isSubscribed() == Result.True)
            {
                if (!_pnContent.activeInHierarchy)
                {
                    ActivateContent();
                }
            }
        }
        catch(System.Exception e)
        {
            Debug.Log("EXCEPTION: " + e.Message);
        }
#else
        _txtSubInfo.text = "Inside unity editor - can't get subscription info.";
#endif
    }

    void ActivateContent()
    {
        if (_pnContent.activeInHierarchy)
            return;

        // Hide popup.
        _pnSub.GetComponent<RectTransform>().DOAnchorPosX(-1000f, .75f)
            .SetEase(Ease.Linear)
            .OnComplete(() => { _pnSub.SetActive(false); });

        _pnContent.SetActive(true);
    }

    public override void UpdateText(Product product)
    {
        switch (product.definition.id)
        {
            case GlobalVariables._monthlySubID:
                _txtMonthlyTitle.text = product.metadata.localizedTitle.FixIAPTitle();
                _txtMonthlyPrice.text = product.metadata.localizedPriceString;
                break;
            case GlobalVariables._yearlySubID:
                _txtYearlyTitle.text = product.metadata.localizedTitle.FixIAPTitle();
                _txtYearlyPrice.text = product.metadata.localizedPriceString;
                break;
        }
    }
#endregion
}