using UnityEngine;

 

public class DontDestroySingleObject : MonoBehaviour{

    //インスタンスが存在するか？

    static bool existsInstance = false;

    void Awake(){

        if(existsInstance){

            Destroy(gameObject);

            return;

        }

        existsInstance = true;

        DontDestroyOnLoad(gameObject);
    }

}