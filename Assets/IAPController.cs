using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class IAPController : MonoBehaviour, IStoreListener
{
    private static IStoreController _StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider _StoreExtensionProvider; // The store-specific Purchasing subsystems.

    #region Variables
    private Product _Product = null;

    [Header("IAP Buttons")]
    [SerializeField] Button _btnMonthlySub = null;
    [SerializeField] Button _btnYearlySub = null;
    [SerializeField] Button _btnBananas = null;
    #endregion

    void Start()
    {
        _btnMonthlySub.onClick.AddListener(() => { BuyProductID(GlobalVariables._monthlySubID); });
        _btnYearlySub.onClick.AddListener(() => { BuyProductID(GlobalVariables._yearlySubID); });
        _btnBananas.onClick.AddListener(() => { BuyProductID(GlobalVariables._bananasID); });

        if (_StoreController == null)
            InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(GlobalVariables._monthlySubID, ProductType.Subscription);
        builder.AddProduct(GlobalVariables._yearlySubID, ProductType.Subscription);
        builder.AddProduct(GlobalVariables._bananasID, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return _StoreController != null && _StoreExtensionProvider != null;
    }

    public void CompletePurchase()
    {
        if (_Product == null)
            Debug.Log("Cannot complete purchase, product not initialized.");
        else
        {
            _StoreController.ConfirmPendingPurchase(_Product);
            Debug.Log("Completed purchase with " + _Product.transactionID.ToString());
        }
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = _StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product:" + product.definition.id.ToString()));
                _Product = product;
                _StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");

        _StoreController = controller;
        _StoreExtensionProvider = extensions;

        for (int i = 0; i < _StoreController.products.all.Length; ++i)
        {
            if (_StoreController.products.all[i].definition.type == ProductType.Subscription)
            {
                SubscriptionHandler.instance.UpdateText(_StoreController.products.all[i]);
                SubscriptionHandler.instance.GetInfo(_StoreController.products.all[i]);
            }
            else if( _StoreController.products.all[i].definition.type == ProductType.Consumable )
            {
                ConsumableHandler.instance.UpdateText(_StoreController.products.all[i]);
                ConsumableHandler.instance.GetInfo(_StoreController.products.all[i]);
            }
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        _Product = args.purchasedProduct;

        switch(_Product.definition.type)
        {
            case ProductType.Subscription:
                SubscriptionHandler.instance.PurchaseSuccess(_Product);
                break;
            case ProductType.Consumable:
                ConsumableHandler.instance.PurchaseSuccess(_Product);
                break;
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        switch (product.definition.type)
        {
            case ProductType.Subscription:
                SubscriptionHandler.instance.PurchaseFailed(product, failureReason);
                break;
            case ProductType.Consumable:
                ConsumableHandler.instance.PurchaseFailed(product, failureReason);
                break;
        }
    }
}