using System;
using UnityEngine;
using System.Threading;

public class DispatchTest : MonoBehaviour
{
    const int width = 1920;
    const int height = 1080;
    //compute shader周り
    [SerializeField] ComputeShader cs;
    private ComputeBuffer computeBuffer = null;
    private int kernelHeavyFunc;
    //その他
    private int flamecnt;
    private int n;
    private int[] host;
    private int lasttime;
    private int deltatime;

    private int time12, time23, time34;
    private float avgdeltatime;

    [SerializeField] int loopnum = 600;//強いGPUならもっと増やして

    private void Awake()
    {
        //Application.targetFrameRate = 60;
        flamecnt = 0;
        avgdeltatime = 0;
        lasttime = Gettime();

        n = width * height;
        computeBuffer = new ComputeBuffer(n, 4);
        host = new int[n];

        kernelHeavyFunc = cs.FindKernel("HeavyFunc");
        cs.SetInt("WX", width);
        cs.SetInt("WY", height);
        cs.SetBuffer(kernelHeavyFunc, "buffer", computeBuffer);
    }


    private void Update()
    {
        var time1 = Gettime();

        cs.SetInt("loopnum", loopnum);
        cs.Dispatch(kernelHeavyFunc, 1, height, 1);
        GL.Flush();

        var time2 = Gettime();

        Thread.Sleep(50);//50ms待つ。CPUの重い処理を想定

        var time3 = Gettime();

        computeBuffer.GetData(host, 0, 0, 4);//GPUの計算が終わっていれば一瞬のはず

        var time4 = Gettime();

        time12 = time2 - time1;
        time23 = time3 - time2;
        time34 = time4 - time3;


        deltatime = time3 - lasttime;
        lasttime = time3;
        flamecnt++;
    }


    private void OnDestroy()
    {
        computeBuffer.Release();
    }

    void OnGUI()
    {
        avgdeltatime = avgdeltatime * 0.99f + 0.01f * deltatime;
        GUILayout.Label($"flamecnt:\t{flamecnt}");
        GUILayout.Label($"1フレームのtime:\t{deltatime}ms");
        GUILayout.Label($"平均FPS:\t{1000.0f / avgdeltatime}");
        GUILayout.Label($"time12:\t{time12}ms");
        GUILayout.Label($"time23:\t{time23}ms");
        GUILayout.Label($"time34:\t{time34}ms");

        // グラフィックデバイス名
        GUILayout.Label($"グラフィックデバイス名:{UnityEngine.SystemInfo.graphicsDeviceName}");
        // グラフィックスAPIタイプ（Direct3D11とか）
        var s0 = UnityEngine.SystemInfo.graphicsDeviceVersion;
        GUILayout.Label($"グラフィックスデバイスバージョン:{s0}");
        // シェーダレベル
        GUILayout.Label($"シェーダレベル:{UnityEngine.SystemInfo.graphicsShaderLevel}");
        // コンピュートシェーダが使えるか
        GUILayout.Label($"コンピュートシェーダが使えるか:{UnityEngine.SystemInfo.supportsComputeShaders}");
    }


    //現在の時刻をms単位で取得
    int Gettime()
    {
        return DateTime.Now.Millisecond + DateTime.Now.Second * 1000
            + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Hour * 60 * 60 * 1000;
    }
}
