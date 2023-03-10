using System;
using UnityEngine;
using System.Threading;

public class DispatchTest : MonoBehaviour
{
    const int width = 1920;
    const int height = 1080;
    //compute shader����
    [SerializeField] ComputeShader cs;
    private ComputeBuffer computeBuffer = null;
    private int kernelHeavyFunc;
    //���̑�
    private int flamecnt;
    private int n;
    private int[] host;
    private int lasttime;
    private int deltatime;

    private int time12, time23, time34;
    private float avgdeltatime;

    [SerializeField] int loopnum = 600;//����GPU�Ȃ�����Ƒ��₵��

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

        Thread.Sleep(50);//50ms�҂BCPU�̏d��������z��

        var time3 = Gettime();

        computeBuffer.GetData(host, 0, 0, 4);//GPU�̌v�Z���I����Ă���Έ�u�̂͂�

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
        GUILayout.Label($"1�t���[����time:\t{deltatime}ms");
        GUILayout.Label($"����FPS:\t{1000.0f / avgdeltatime}");
        GUILayout.Label($"time12:\t{time12}ms");
        GUILayout.Label($"time23:\t{time23}ms");
        GUILayout.Label($"time34:\t{time34}ms");

        // �O���t�B�b�N�f�o�C�X��
        GUILayout.Label($"�O���t�B�b�N�f�o�C�X��:{UnityEngine.SystemInfo.graphicsDeviceName}");
        // �O���t�B�b�N�XAPI�^�C�v�iDirect3D11�Ƃ��j
        var s0 = UnityEngine.SystemInfo.graphicsDeviceVersion;
        GUILayout.Label($"�O���t�B�b�N�X�f�o�C�X�o�[�W����:{s0}");
        // �V�F�[�_���x��
        GUILayout.Label($"�V�F�[�_���x��:{UnityEngine.SystemInfo.graphicsShaderLevel}");
        // �R���s���[�g�V�F�[�_���g���邩
        GUILayout.Label($"�R���s���[�g�V�F�[�_���g���邩:{UnityEngine.SystemInfo.supportsComputeShaders}");
    }


    //���݂̎�����ms�P�ʂŎ擾
    int Gettime()
    {
        return DateTime.Now.Millisecond + DateTime.Now.Second * 1000
            + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Hour * 60 * 60 * 1000;
    }
}
