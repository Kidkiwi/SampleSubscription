using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using DG.Tweening;

public class SubscriptionHandler : MonoBehaviour
{
    public static SubscriptionHandler instance { get; set; }

    #region Variables
    const string _monthlySubID = "m_subscription";
    const string _yearlySubID = "y_subscription";

    [SerializeField] TextMeshProUGUI _txtSubInfo = null;
    [SerializeField] TextMeshProUGUI _txtMessage = null;
    [SerializeField] GameObject _pnSub = null;
    [SerializeField] GameObject _pnContent = null;

    [Header("IAP Buttons")]
    [SerializeField] IAPButton _btnMonthlySub = null;
    [SerializeField] IAPButton _btnYearlySub = null;

    List<SubscriptionInfo> _lstSubscriptionInfo = new List<SubscriptionInfo>();
    #endregion

    #region Functions
    private void Awake()
    {
        instance = this;

        _btnMonthlySub.onPurchaseComplete.AddListener((v) => { PurchaseSuccess(v); });
        _btnYearlySub.onPurchaseComplete.AddListener((v) => { PurchaseSuccess(v); });

        _btnMonthlySub.onPurchaseFailed.AddListener((v, v2) => { PurchaseFailed(v, v2); });
        _btnYearlySub.onPurchaseFailed.AddListener((v, v2) => { PurchaseFailed(v, v2); });

        _txtMessage.text = "";
    }

    public void PurchaseSuccess(Product product)
    {
        switch(product.definition.id)
        {
            case _monthlySubID:
                _txtMessage.text = "Monthly sub purchased!";
                break;
            case _yearlySubID:
                _txtMessage.text = "Yearly sub purchased!";
                break;
        }

        if (product.definition.type == ProductType.Subscription)
            GetInfo(product);

#if UNITY_EDITOR
        ActivateContent();
#endif

        _txtMessage.color = Color.green;
    }

    public void PurchaseFailed(Product product, PurchaseFailureReason purchaseFailureReason)
    {
        Debug.Log("Purchase failed: " + product.definition.id);

        switch (product.definition.id)
        {
            case _monthlySubID:
                _txtMessage.text = "Monthly sub failed!\n" + purchaseFailureReason;
                break;
            case _yearlySubID:
                _txtMessage.text = "Yearly sub failed!\n" + purchaseFailureReason;
                break;
        }

        _txtMessage.color = Color.red;

        // Just in case content is active.
        _pnSub.SetActive(true);
        _pnContent.SetActive(false);
    }

    public void GetInfo(Product p)
    {
#if !UNITY_EDITOR
        SubscriptionManager subscriptionManager = new SubscriptionManager(p, null);

        if( subscriptionManager.getSubscriptionInfo() != null )
        {
            _lstSubscriptionInfo.Insert(0, subscriptionManager.getSubscriptionInfo());

            _txtSubInfo.text += "<color=#ff0>Purchase Date:</color> " + _lstSubscriptionInfo[0].getPurchaseDate() + "\n"
                + "<color=#ff0>Auto Renewing:</color> " + _lstSubscriptionInfo[0].isAutoRenewing() + "\n"
                + "<color=#ff0>Free Trial:</color> " + _lstSubscriptionInfo[0].isFreeTrial() + "\n"
                + "<color=#ff0>Is Subscriped:</color> " + _lstSubscriptionInfo[0].isSubscribed() + "\n"
                + "-----------------------\n";
        }

        // We check if our subscription still has remaining time, this makes sure even if the user
        // cancels but still has time left, that they get the content.
        if( _lstSubscriptionInfo.Count > 0 && _lstSubscriptionInfo[0].getRemainingTime().TotalSeconds > 0 )
        {
            if( !_pnContent.activeInHierarchy )
            {
                ActivateContent();
            }
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
#endregion
}