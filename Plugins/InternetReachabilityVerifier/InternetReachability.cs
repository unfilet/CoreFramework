using Localized;
using UI;
using UnityEngine;

using IRVerifier = InternetReachabilityVerifier;

[RequireComponent(typeof(IRVerifier))]
public sealed class InternetReachability : Singleton<InternetReachability>
{
    public IRVerifier.Status Status => Verifier.status;
    public static bool NetVerified => Instance.Status == IRVerifier.Status.NetVerified;

    public static bool Reachable => Instance.Status == IRVerifier.Status.NetVerified || Instance.Status == IRVerifier.Status.PendingVerification;

    public event IRVerifier.StatusChangedDelegate OnStatusChanged
    {
        add => Verifier.statusChangedDelegate += value;
        remove => Verifier.statusChangedDelegate -= value;
    }

    private IRVerifier Verifier = null;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        Verifier = this.gameObject.GetOrAddComponent<IRVerifier>();
        Verifier.forceReverification();
        Verifier.statusChangedDelegate += netStatusChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Instance != this || Verifier == null ) return;
        Verifier.statusChangedDelegate -= netStatusChanged;
    }

    private void netStatusChanged(IRVerifier.Status newStatus)
    {
        Debug.Log("netStatusChanged: new IRVerifier.Status = " + newStatus);
    }


    public static bool CheckConnection(bool showAlert = false) {

        if (!Reachable) {
            //if (showAlert)
                //AlertController.Show(
                    //new AlertView.Data(message: LocalizationManager.Instance["common.internet"])
                    //);

            return false;
        }

        return true;
    }

}
