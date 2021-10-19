using UnityEngine;

using IRVerifier = InternetReachabilityVerifier;


[System.Serializable]
public class InternetReachabilityException : System.Exception
{
    public static readonly InternetReachabilityException Default = new InternetReachabilityException();
    
    private const string message = "No internet connection.\nPlease check your internet connection or try again later";

    public InternetReachabilityException() : base(message)
    {}
}

public static class InternetReachabilityExt
{
    public static bool IsNetVerified(this IRVerifier @this)
        => @this.status == IRVerifier.Status.NetVerified;

    public static bool IsReachable(this IRVerifier @this)
        => @this.status == IRVerifier.Status.NetVerified ||
           @this.status == IRVerifier.Status.PendingVerification;
}
