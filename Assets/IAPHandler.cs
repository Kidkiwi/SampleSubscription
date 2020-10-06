using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPHandler: MonoBehaviour
{
    public virtual void PurchaseSuccess(Product product)
    {
        Debug.Log("Purchase of product: " + product.metadata.localizedTitle + " was a success");
    }

    public virtual void UpdateText(Product product)
    {
        Debug.Log("Updating text of product: " + product.metadata.localizedTitle + " was a success");
    }

    public virtual void GetInfo(Product product)
    {
        Debug.Log("Getting info of product: " + product.metadata.localizedTitle + " was a success");
    }

    public virtual void PurchaseFailed(Product product, PurchaseFailureReason purchaseFailureReason)
    {
        Debug.Log("Purchase of product: " + product.metadata.localizedTitle + " was a failure");
    }
}