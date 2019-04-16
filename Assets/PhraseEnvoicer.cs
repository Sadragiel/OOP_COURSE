using System.Runtime.InteropServices;

public static class PhraseEnvoicer {

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("MyTTSLib_WIn")]
    public static extern bool InitSystem();

    [DllImport("MyTTSLib_WIn", CharSet = CharSet.Unicode)]
    public static extern void PlayText([MarshalAs(UnmanagedType.LPWStr)]string text);

    [DllImport("MyTTSLib_WIn")]
    public static extern void StopSystem();
#elif UNITY_WEBGL && !UNITY_EDITOR

    public static bool InitSystem(){return true;}

    [DllImport("__Internal")]
    public static extern void PlayText(string text);

    [DllImport("__Internal")]
    public static extern bool CheckAvailable();

    public static void StopSystem(){}

#else
    public static bool InitSystem(){return true;}

    public static void PlayText(string obj){}

    public static void StopSystem(){}
#endif

    public static void PlayTextControlled(string text)
    {
            PhraseEnvoicer.PlayText(text);
    }
}
