// 示例：在脚本中设置粒子Custom Data
using System.Collections.Generic;
using UnityEngine;

public class SetParticleCustomData : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private List<Vector4> customData = new List<Vector4>();

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        // 如果Mesh没有开启Read/Write，下面的操作可能失败
        particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
        Debug.Log(customData.Count + " particles found.");
        // 修改自定义数据
        for (int i = 0; i < customData.Count; i++)
        {
            customData[i] = new Vector4(Random.value, Random.value, 0, 1);
        }

        particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
    }
}