using System;
using System.Collections.Generic;
using UnityEngine;
using CaomaoFramework.Resource;
namespace CaomaoFramework.Effect
{
    /// <summary>
    /// 特效类型0=>普通，1=>飞行
    /// </summary>
    public enum EffectType : byte
    {
        Normal = 0,
        Flying = 1
    }
    public class EffectHandler : MonoBehaviour
    {
        #region 字段
        private Dictionary<string, Transform> m_locationDic = new Dictionary<string, Transform>();
        private Dictionary<int, Dictionary<int, GameObject>> m_fxDic = new Dictionary<int, Dictionary<int, GameObject>>();
        private Dictionary<int, List<int>> m_groupFXList = new Dictionary<int, List<int>>();
        private static Dictionary<string, AnimationClip> m_animationClips = new Dictionary<string, AnimationClip>();
        private Renderer[] m_renderer;
        private Material[] m_mat;
        private int currentShaderFx = 0;
        private static HashSet<string> m_loadedFX = new HashSet<string>();
        #endregion
        #region 属性
        #endregion
        #region 构造方法
        #endregion
        void Awake()
        {
            GetMaterials();
        }
        /// <summary>
        /// 插入飞行特效
        /// </summary>
        /// <param name="fx"></param>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="distance"></param>
        public void Shoot(EffectData fx, Transform target, float speed = 10, float distance = 30)
        {

        }
        /// <summary>
        /// 插入特效
        /// </summary>
        /// <param name="id"></param>
        /// <param name="target"></param>
        /// <param name="action"></param>
        /// <param name="bone_path"></param>
        public void HandleFx(int id, Transform target = null, Action<GameObject, int> action = null, string bone_path = "")
        {
            if (EffectData.dataMap.ContainsKey(id))
            {
                var effectData = EffectData.dataMap[id];
                if (effectData.effectType == EffectType.Flying)
                {
                    Shoot(effectData, target);
                }
                else
                {
                    PlayFx(id, effectData, action, bone_path);
                }
            }
        }
        public void RemoveFx(int id)
        {
            if (id == 0)
            {
                return;
            }
            var fx = EffectData.dataMap[id];
            RemoveFx(id, fx.group);
            if (id == currentShaderFx)
            {

            }
        }
        public void RemoveFx(int id, int group)
        {

        }
        /// <summary>
        /// 取得物体的所有renderer和materials
        /// </summary>
        private void GetMaterials()
        {
            var renderers = new List<Renderer>();
            var skinMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>(true);
            var meshRenderer = GetComponentsInChildren<MeshRenderer>(true);
            renderers.AddRange(skinMeshRenderer);
            renderers.AddRange(meshRenderer);
            if (renderers.Count != 0)
            {
                this.m_renderer = renderers.ToArray();
                this.m_mat = new Material[this.m_renderer.Length];
                for (int i = 0; i < this.m_renderer.Length; i++)
                {
                    m_mat[i] = this.m_renderer[i].material;
                }
            }
            else
            {
                this.m_renderer = null;
                this.m_mat = null;
            }
        }
        /// <summary>
        /// 插入特效
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fx"></param>
        /// <param name="action"></param>
        /// <param name="bone_path"></param>
        private void PlayFx(int id, EffectData fx, Action<GameObject, int> action = null, string bone_path = "")
        {

        }
        /// <summary>
        /// 处理特效动画播放
        /// </summary>
        /// <param name="fx"></param>
        private void HandleAnim(EffectData fx)
        {
            if (!string.IsNullOrEmpty(fx.anim))
            {
                if (!gameObject.GetComponent<Animation>())
                {
                    gameObject.AddComponent<Animation>();
                }
                //如果包含的话就直接播放动画
                if (m_animationClips.ContainsKey(fx.anim))
                {
                    gameObject.GetComponent<Animation>().AddClip(m_animationClips[fx.anim], fx.anim);
                    gameObject.GetComponent<Animation>().Play(fx.anim);
                }
                //否则就加载然后播放动画
                else
                {
                    ResourceManager.singleton.LoadEffect(fx.resourcePath, new AssetRequestFinishedEventHandler((_assetRequest) =>
                    {
                        m_animationClips[fx.anim] = _assetRequest.AssetResource.MainAsset as AnimationClip;
                        gameObject.GetComponent<Animation>().AddClip(m_animationClips[fx.anim], fx.anim);
                        gameObject.GetComponent<Animation>().Play(fx.anim);
                    }));
                }
            }
        }
        /// <summary>
        /// 处理淡入淡出
        /// </summary>
        /// <param name="fx"></param>
        private void HandleFade(EffectData fx)
        {
            if (fx.fadeStart != fx.fadeEnd)
            {
                try
                {
                    if (this.m_mat != null)
                    {
                        var color = GetMatColor("_Color");
                        if (color.a == fx.fadeEnd)//如果淡出的alpha为0，就隐藏renderer
                        {
                            if (this.m_renderer != null && fx.fadeEnd == 0)
                            {
                                this.SetRendererEnable(false);
                            }
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                var startTime = Time.realtimeSinceStartup;
                var repeatTimer = FrameTimerManager.AddTimer((uint)fx.fadeDelay, 100, () =>
                {
                    if (this.m_mat != null)
                    {
                        var deltaTime = Time.realtimeSinceStartup - startTime;//偏移的时间
                        var target = fx.fadeStart + ((fx.fadeStart - fx.fadeEnd) * deltaTime * 1000 / fx.fadeDuration);
                        this.SetMatColor("_Color", new Color(1, 1, 1, target));//设置成白色
                    }
                });
                FrameTimerManager.AddTimer((uint)fx.fadeDelay + (uint)fx.fadeDuration, 0, () =>
                {
                    FrameTimerManager.DelTimer(repeatTimer);
                    if (this.m_renderer != null && fx.fadeEnd == 0)
                    {
                        this.SetRendererEnable(false);
                    }
                });
            }
        }
        /// <summary>
        /// 获得第一个材质的颜色
        /// </summary>
        /// <param name="prop">材质属性名称</param>
        /// <returns></returns>
        private Color GetMatColor(string prop)
        {
            if (this.m_mat != null && this.m_mat.Length != 0)
            {
                return this.m_mat[0].GetColor(prop);
            }
            else
            {
                return Color.clear;//设置成全黑色全透明Color.cleat==new Color(0,0,0,0);
            }
        }
        /// <summary>
        /// 取得材质的shader
        /// </summary>
        /// <returns></returns>
        private Shader GetMatShader()
        {
            if (m_mat != null && m_mat.Length > 0)
            {
                return m_mat[0].shader;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据特定的shader取得材质的shader
        /// </summary>
        /// <param name="shader"></param>
        /// <returns></returns>
        private Shader GetMatShader(Shader shader)
        {
            if (m_mat != null && shader != null)
            {
                return this.m_mat[0].shader;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 设置材质的颜色
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        private void SetMatColor(string prop, Color value)
        {
            if (this.m_mat != null)
            {
                foreach (var item in this.m_mat)
                {
                    item.SetColor(prop, value);
                }
            }
        }
        /// <summary>
        /// 设置所有材质的shader为特定的shader
        /// </summary>
        /// <param name="shader"></param>
        private void SetMatShader(Shader shader)
        {
            if (m_mat != null && shader != null)
            {
                foreach (var mat in m_mat)
                {
                    mat.shader = shader;
                }
            }
        }
        /// <summary>
        /// 设置材质的数值
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        private void SetMatFloat(string prop, float value)
        {
            if (this.m_mat != null)
            {
                foreach (var item in this.m_mat)
                {
                    item.SetFloat(prop, value);
                }
            }
        }
        /// <summary>
        /// 设置材质的贴图
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="texture"></param>
        private void SetMatTexture(string prop, Texture texture)
        {
            foreach (var item in this.m_mat)
            {
                item.SetTexture(prop, texture);
            }
        }
        /// <summary>
        /// 设置Renderer是否可见
        /// </summary>
        /// <param name="value"></param>
        private void SetRendererEnable(bool value)
        {
            foreach (var item in this.m_renderer)
            {
                item.enabled = value;
            }
        }
    }
}
