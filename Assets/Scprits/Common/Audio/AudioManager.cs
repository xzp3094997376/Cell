using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace future3d.unityLibs
{
    /// <summary>
    /// 音频控制管理类
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        public void Awake()
        {
            Instance = this;

            //设置音频控制对象初始位置
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// 从Resources加载音频（背景音乐）资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AudioClip LoadMusic(string name)
        {
            return Resources.Load(name, typeof(AudioClip)) as AudioClip;
        }

        /// <summary>
        /// 从Resources加载音频（音效）资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AudioClip LoadSound(string name)
        {
            return Resources.Load(name, typeof(AudioClip)) as AudioClip;
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayMusic(string name)
        {

            AudioClip clip = LoadMusic(name);

            GameObject obj = new GameObject("AudioMusic::" + clip.name);
            obj.transform.parent = transform;

            AudioSource source = obj.AddComponent(typeof(AudioSource)) as AudioSource;
            source.clip = clip;
            source.loop = true;
            source.playOnAwake = false;
            source.Play();

        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name"></param>
        public void PlaySound(string name)
        {

            AudioClip clip = LoadSound(name);

            GameObject obj = new GameObject("AudioSound::" + clip.name);
            obj.transform.parent = transform;

            AudioSource source = obj.AddComponent(typeof(AudioSource)) as AudioSource;
            source.clip = clip;
            source.Play();
            source.playOnAwake = false;
            Destroy(obj, clip.length); //播放完销毁

        }

        /// <summary>
        /// 关闭所有音频
        /// </summary>
        public void StopMusic()
        {
            List<GameObject> al = new List<GameObject>();

            foreach (Transform child in transform)
            {
                if (child.name.StartsWith("AudioMusic::"))
                {
                    al.Add(child.gameObject);
                }
            }

            int count = al.Count;
            for (int i = 0; i < count; ++i)
            {
                Destroy(((GameObject)al[i]));
            }
        }
    }
}
