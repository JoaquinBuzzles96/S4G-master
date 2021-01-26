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
    public GameObject prop; //utilizaremos esta variable para saber que prop estamos usando, en un futuro sera un array (quiza) de momento con uno basta

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

            //Debug.Log($"Vamos a cargar el audio {real_audio_id} de la enfermera {currentNurse}");
        }

        return real_audio_id;
    }

    public void UpdateNurseName()
    {
        UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().canvasName.text = $"Endoscopy \n Nurse {currentNurse}";
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
        //TODO: 
        // test si funciona modificar el path de la enfermera para que a veces salga y a veces no
        // eliminar herramienta de la mano y lanzarla contra la enfermera
        // testear si funciona Hacer despaarecer el prop

        SimpleWaypointMovement movementNurse;
        string anim = "";
        Vector3 position = Vector3.zero; //usaremos esta variable para asignar la posicion de la mano del personaje que sea
        //SimpleWaypointMovement movementNurse = null;
        //adaptar para que sirva para todos los casos
        switch (dialogue_id)
        {
            
            case "D3.1":
                movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movementNurse.isNurse = true;
                movementNurse.exitRoom = true;
                movementNurse.firstTime = true;
                movementNurse.canMove = true;
                playingAnimation = true;
                break; 
            case "D5.1":
                movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movementNurse.isNurse = true;
                movementNurse.exitRoom = true;
                movementNurse.firstTime = true;
                movementNurse.canMove = true;
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
                break;
            case "D1.1.1":
            case "D1.1.2":
            case "D1.1.3":
            case "D1.1.4":
                //Solo de prueba: //en cualquiera de estos el anestesiologo se vuelve
                //StartCoroutine(TurnsTo(UI_Manager.Instance.dictionaryCharacteres["Anaesthesiologist"].gameObject, UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.transform.position));
                break;

            case "D1.1": //poner endoscopio en la mano del endoescopista 1
                position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos.position; 
                SetProp(position, prop);// lo ponemos en tu mano
                break;
            case "D3.2": //entra una enfermera (dividir la anim que esta hecha en dos)
                UpdateNurseName();
                movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movementNurse.isNurse = true;
                movementNurse.exitRoom = false;
                movementNurse.firstTime = true;
                movementNurse.canMove = true;
                playingAnimation = true;
                break;
            case "D5.2": //entra una enfermera (dividir la anim que esta hecha en dos)
                UpdateNurseName();
                movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movementNurse.isNurse = true;
                movementNurse.exitRoom = false;
                movementNurse.firstTime = true;
                movementNurse.canMove = true;
                playingAnimation = true;
                break;
            case "D6.2": //la enfermera te da el lazo, extraes el endoscopio y se lo das a la enfermera y la enfermera lo deja en la mesa
                //me da el lazo
                anim = "Give"; // en las de dar podemos ahcer que mire a un target
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));
                position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos.position;
                SetProp(position, prop);//lo ponemos en mi mano y lo eliminamos de la suya
                //TODO: extraigo endoscopio

                //se los das a la enfermera
                anim = "Give";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim));
                position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos.position;
                SetProp(position, prop);// lo ponemos en su mano
                // lo deja en la mesa:
                //TODO: Ir a la mesa y elimnarlo de su mano
                //Posicion = mesa.transform
                SetProp(position, prop);
                break;
            case "D6.4": //rebusca en la mesa y te da una herramienta erronea
                //TODO: Ir a la mesa
                anim = "LookFor";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));
                position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos.position;
                SetProp(position, prop);// lo ponemos en su mano
                anim = "Give";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));
                position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos.position;
                SetProp(position, prop);// lo ponemos en tu mano
                break;
            case "D6.1.1": //lanzas la herramienta de vuelta
                anim = "Throw";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim));
                //TODO: eliminar herramienta de la mano y lanzarla contra la enfermera
                DeleteProp(prop);
                break;
            case "D6.1.2": //le devuelves la herramienta
            case "D6.1.3":
                anim = "Give";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim));
                //TODO: Hacer despaarecer el prop
                DeleteProp(prop);
                break;
            case "D6.4.2": // el endoescopista 2 ayuda a la enfermera a buscar el forceps en la mesa
                //Ir a la mesa ambos
                //Anim de buscar:
                anim = "LookFor";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject, anim));
                //poner el prop en la mano de la enfermera
                SetProp(position, prop);

                break;
            case "D7.1": //la enfermera te pasa el forceps
                anim = "Give";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));
                DeleteProp(prop); //igual es mejor llamar a esto desde la corutina y que desaparezca a mitad de la animacion
                break;
            case "D8.1": //la enfermera busca enrviosa la herramienta y se le cae al suelo
                anim = "LookFor";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));
                //TODO: Animacion de caer al suelo
                break;
            case "D10.1": //la segunda enfermera viene y ayuda a la primera enfermera
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

    public void SetProp(Vector3 coord, GameObject prop)
    {
        prop.SetActive(true);
        prop.transform.position = coord;
    }

    public void DeleteProp(GameObject prop)
    {
        prop.SetActive(false);
    }






}
