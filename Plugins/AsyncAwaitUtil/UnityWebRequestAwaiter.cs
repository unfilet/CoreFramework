using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestAwaiter : INotifyCompletion
{
	private UnityWebRequestAsyncOperation asyncOp;
	private Action continuation;


	public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
	{
		this.asyncOp = asyncOp;
		asyncOp.completed += OnRequestCompleted;
	}

	public bool IsCompleted { get { return asyncOp.isDone; } }

	public UnityWebRequest GetResult() {
        Assert(IsCompleted);
        return asyncOp.webRequest;
    }

	public void OnCompleted(Action continuation)
	{
        Assert(this.continuation == null);

        this.continuation = continuation;

        if (IsCompleted)
            continuation?.Invoke();
    }

    private void OnRequestCompleted(AsyncOperation obj)
	{
		continuation?.Invoke();
        asyncOp.completed -= OnRequestCompleted;
    }

    static void Assert(bool condition)
    {
        if (!condition)
            throw new Exception("Assert hit in UnityAsyncUtil package!");
    }
}

public static class UnityWebRequestTextureWraper
{
    public static async Task<Texture2D> GetTextureAsync(string uri)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(uri))
        {
            await uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityWebRequest.Result.ProtocolError)
                throw new Exception(uwr.error);
            else
                return DownloadHandlerTexture.GetContent(uwr);
        }
    }
}

public static class ExtensionMethods
{
	public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
	{
		return new UnityWebRequestAwaiter(asyncOp);
	}
}

/*
// Usage example:
UnityWebRequest www = new UnityWebRequest();
// ...
await www.SendWebRequest();
Debug.Log(req.downloadHandler.text);
*/