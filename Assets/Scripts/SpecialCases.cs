using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCases : MonoBehaviour
{

    private static SpecialCases instance = null;

    public static SpecialCases Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    //Esta clase la utilizaremos para definir todas las variables que usaremos en los casos "especiales" :')

    #region Case3
    public int currentNurse = 1;
    public bool playingAnimation = false;
    #endregion

    public string ChechkAudio(string audio_id, string _speaker) 
    {
        string real_audio_id = audio_id;
        //Debug.Log($"Vamos a comprobar el audio del speaker {_speaker}");
        if (_speaker == "EndoscopyNurse")//tenemos que comprobar cual de ellas es :')
        {
            if (currentNurse == 2)
            {
                real_audio_id = audio_id + "2";
            }
            else if (currentNurse == 3)
            {
                real_audio_id = audio_id + "3"; ;
            }

            Debug.Log($"Vamos a cargar el audio {real_audio_id} de la enfermera {currentNurse}");
        }

        return real_audio_id;
    }

    public void CheckSituation(string situation_id) //SITUATION 5 AND 3
    {
        //Check if nurse cry
        if (situation_id == "S3_4" || situation_id == "S5")
        {
            currentNurse++;
            Debug.Log($"Cambiamos a la enfermera {currentNurse}");
        }
    }

    //devolver float
    public void CheckSpecialEvent(string dialogue_id)//con esto comprobaremos si en el dialogo actual debe darse alguna situacion en concreto
    {
        //adaptar para que sirva para todos los casos
        switch (dialogue_id)
        {
            case "D3.1":
                SimpleWaypointMovement movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movementNurse.canMove = true;
                playingAnimation = true;
                break; 
            case "D5.1":
                SimpleWaypointMovement movementNurse2 = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movementNurse2.canMove = true;
                playingAnimation = true;
                break;
            case "D11.1": //dialogo en el que entra el secretario
                SimpleWaypointMovement movement = Case3Resources.Instance.secretary.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movement.canMove = true;
                playingAnimation = true;
                break;
            case "D1.4":
                //Esta es solo para probar, no pasa realmente: //este es el dialogo en el que el anestesiologo se gira hacia la enfermera de endoscopia
                //StartCoroutine(TurnsTo(UI_Manager.Instance.dictionaryCharacteres["Anaesthesiologist"].gameObject, UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.transform.position));
                string anim = "Throw";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));


                break;
            case "D1.1.1":
            case "D1.1.2":
            case "D1.1.3":
            case "D1.1.4":
                //Solo de prueba: //en cualquiera de estos el anestesiologo se vuelve
                //StartCoroutine(TurnsTo(UI_Manager.Instance.dictionaryCharacteres["Anaesthesiologist"].gameObject, UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.transform.position));
                break;

        }

    }

    IEnumerator TurnsTo(GameObject characer, Vector3 targetPosition) //para volver a la posicion original basta con indicar que se giren hacia en endoescopista 1
    {
        playingAnimation = true;
        //activar aniamcion de girar (hay que comprobar hacia donde gira y en funcion de eso poner derecha o izquierda)
        Animator animator = characer.GetComponent<Animator>();
        float turnAnimDuration = 0f;
        float rotationSpeed = 1f;
        if (targetPosition.x < characer.transform.position.x && changed(characer.transform.position.y, targetPosition.y))
        {
            Debug.Log("Movemos a la derecha");
            //animator.SetBool("turnRight", true);
            //turnAnimDuration = GetDuration("RightTurn", animator);
        }
        else if (changed(characer.transform.position.y, targetPosition.y))
        {
            Debug.Log("Movemos a la izquierda");
            //animator.SetBool("turnLeft", true);
            //turnAnimDuration = GetDuration("LeftTurn", animator);
        }

        Debug.Log($"Almacenamos la duracion de la animación actual de girarase = {turnAnimDuration}");

        //Opcion 1 (con animacion haciendo cosas raras)
        //yield return new WaitForSeconds(turnAnimDuration);
        //characer.transform.LookAt(targetPosition);


        //Opcion 2 (sin animacion)
        while (Vector3.Distance(characer.transform.forward, targetPosition - characer.transform.position) > 0.2f)//characer.transform.rotation != Quaternion.LookRotation(targetPosition - characer.transform.position)
        {
            Debug.Log($"Seguimos girando hasta que encontremos al player");
            characer.transform.rotation = Quaternion.Lerp(characer.transform.rotation, Quaternion.LookRotation(targetPosition - characer.transform.position), Time.deltaTime * rotationSpeed);
            yield return null;
        }

        playingAnimation = false;
    }

    IEnumerator PlaySimpleAnim(GameObject character, string animName)
    {
        playingAnimation = true;
        //IMPORTANTE: La animacion y la transicion deben de tener el mismo nombre para que funcione bien
        //Si se queda en bucle la animacion revisar si se ha añadido el animManager y el false a la animacion dentro del mismo 
        Animator animator = character.GetComponent<Animator>();
        animator.SetBool(animName, true);
        float animDuration = GetDuration(animName, animator);

        yield return new WaitForSeconds(animDuration);

        playingAnimation = false;
    }

    public float GetDuration(string animationName, Animator anim)
    {
        float duration = 0f;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (animationName == clip.name)
            {
                duration = clip.length;
                break;
            }
        }

        return duration;
    }

    bool changed(float a, float b)
    {
        if ((int)a == (int)b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public GameObject SetProp(Vector3 coord, GameObject prop)
    {
        return Instantiate(prop, coord, prop.transform.rotation);
    }

    public void DeleteProp(GameObject prop)
    {
        Destroy(prop);
    }






}
