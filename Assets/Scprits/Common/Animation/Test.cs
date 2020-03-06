using UnityEngine;

public class Test : MonoBehaviour
{
    public Animation ani;
    public Animator ator;
    private void Start1()
    {
        AnimatorEventComponent acom = ator.gameObject.AddComponent<AnimatorEventComponent>();
        acom.AddEvent("cube2", 29, () =>
        {
            ator.speed = 0f;
            Debug.LogError("fdsfs");
        });
    }

    //public TestScriptableAssets data;
    //private void OnEnable()
    //{
    //    if (data==null)
    //    {
    //        data = Resources.Load("Data") as TestScriptableAssets;            
    //    }
    //    Debug.Log(data.prefabName);
    //    Debug.Log(data.desStrs[4]);
    //}
}
