using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using DG.Tweening;

public class SubscriptionHandler : MonoBehaviour
{
    #region Variables
    const string _monthlySubID = "m_subscription";
    const string _yearlySubID = "y_subscription";

    [SerializeField] TextMeshProUGUI _txtMessage = null;
    [SerializeField] GameObject _pnSub = null;
    [SerializeField] GameObject _pnContent = null;

    [Header("IAP Buttons")]
    [SerializeField] IAPButton _btnMonthlySub = null;
    [SerializeField] IAPButton _btnYearlySub = null;
    #endregion

    #region Functions
    private void Awake()
    {
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

        _txtMessage.color = Color.green;


        // Hide popup.
        _pnSub.GetComponent<RectTransform>().DOAnchorPosX(-1000f, .75f)
            .SetEase(Ease.Linear)
            .OnComplete(() => { _pnSub.SetActive(false); });

        _pnContent.SetActive(true);
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
    #endregion
}