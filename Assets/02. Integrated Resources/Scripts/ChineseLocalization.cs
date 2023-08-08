using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChineseLocalization : MonoBehaviour
{
    [System.Serializable]
    public class AudioCustomize
    {
        public AudioSource source;
        public AudioClip clip_Us;
        public AudioClip clip_Cn;
    }

    [System.Serializable]
    public class ImageCustomize
    {
        public UnityEngine.UI.Image source;
        public Sprite sprite_Us;
        public Sprite sprite_Cn;
    }

    [System.Serializable]
    public class MaterialCustomize
    {
        public Material source;
        public string[] key;
        public Texture[] sprite_Us;
        public Texture[] sprite_Cn;
    }

    [System.Serializable]
    public class VideoCustomize
    {
        public UnityEngine.Video.VideoPlayer source;
        public UnityEngine.Video.VideoClip video_Us;
        public UnityEngine.Video.VideoClip video_Cn;
    }

    public List<AudioCustomize> listAudio = new List<AudioCustomize>();
    public List<ImageCustomize> listImage = new List<ImageCustomize>();
    public List<MaterialCustomize> listMaterial = new List<MaterialCustomize>();
    public List<VideoCustomize> listVideo = new List<VideoCustomize>();

    // Start is called before the first frame update
    void Start()
    {
        //Custom(GameSettingCtrl.GetLanguageState());
        GameSettingCtrl.AddLocalizationChangedEvent(Custom);
    }

    public void Custom(LanguageState language)
    {
        switch (language)
        {
            case LanguageState.schinese:
                foreach (AudioCustomize i in listAudio)
                {
                    i.source.clip = i.clip_Cn;
                }

                foreach(ImageCustomize i in listImage)
                {
                    i.source.sprite = i.sprite_Cn;
                }

                foreach (MaterialCustomize i in listMaterial)
                {
                    for (int k = 0; k < i.key.Length; k++)
                    {
                        i.source.SetTexture(i.key[k], i.sprite_Cn[k]);
                    }
                }

                foreach (VideoCustomize i in listVideo)
                {
                    i.source.clip = i.video_Cn;
                }

                break;
            default:
                foreach (AudioCustomize i in listAudio)
                {
                    i.source.clip = i.clip_Us;
                }

                foreach (ImageCustomize i in listImage)
                {
                    i.source.sprite = i.sprite_Us;
                }

                foreach (MaterialCustomize i in listMaterial)
                {
                    for (int k = 0; k < i.key.Length; k++)
                    {
                        i.source.SetTexture(i.key[k], i.sprite_Us[k]);
                    }
                }
                foreach (VideoCustomize i in listVideo)
                {
                    i.source.clip = i.video_Us;
                }
                break;
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
