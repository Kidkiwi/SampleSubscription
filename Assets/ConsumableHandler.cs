using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;

public class ConsumableHandler : IAPHandler
{
    public static ConsumableHandler instance { get; set; }

    [Header("IAP Button")]
    [SerializeField] TextMeshProUGUI _txtBananasPrice = null;

    void Awake()
    {
        instance = this;
    }

    public override void PurchaseSuccess(Product product)
    {
        base.PurchaseSuccess(product);

        switch(product.definition.id)
        {
            case GlobalVariables._bananasID:
                GlobalVariables._curBananas++;
                BananaController.BananasUpdated?.Invoke();
                break;
        }
    }

    public override void UpdateText(Product product)
    {
        base.UpdateText(product);

        switch (product.definition.id)
        {
            case GlobalVariables._bananasID:
                _txtBananasPrice.text = product.metadata.localizedPriceString;
                break;
        }
    }

    public override void GetInfo(Product product)
    {
        base.GetInfo(product);

        Debug.Log(product.metadata.localizedDescription);
    }
}
