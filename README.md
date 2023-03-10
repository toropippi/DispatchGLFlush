## Introduction
ComputeShader.Dispatch命令はGPUで計算を行なうためのものです。  
この命令は非同期実行のため、GPUの計算が終わる前にCPUは次の命令にうつります。  
普通に考えればGPUが計算している最中にCPUも計算できてラッキーとなるのですが、実はGPUで計算が始まることも保証してくれません。  

GLFlush()を行なうことで、即時GPUに実行を促します。  