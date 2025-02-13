﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace Michsky.UI.ModernUIPack
{
    public class CustomDropdown : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
    {
        // Resources
        public Animator dropdownAnimator;
        public GameObject triggerObject;
        public TextMeshProUGUI selectedText;
        public Image selectedImage;
        public Transform itemParent;
        public GameObject itemObject;
        public GameObject scrollbar;
        public VerticalLayoutGroup itemList;
        public Transform currentListParent;
        public Transform listParent;
        public AudioSource soundSource;

        // Settings
        public bool enableIcon = true;
        public bool enableTrigger = true;
        public bool enableScrollbar = true;
        public bool setHighPriorty = true;
        public bool outOnPointerExit = false;
        public bool isListItem = false;
        public bool invokeAtStart = false;
        public AnimationType animationType;
        public int selectedItemIndex = 0;
        public bool enableDropdownSounds = false;
        public bool useHoverSound = true;
        public bool useClickSound = true;

        // Saving
        public bool saveSelected = false;
        public string dropdownTag = "Dropdown";

        //Joaquin changes
        [SerializeField]
        public bool isCasesDropdown;

        List<DialogueContainer> cases = new List<DialogueContainer>();

        // Item list
        [SerializeField]
        public List<Item> dropdownItems = new List<Item>();
        [System.Serializable]
        public class DropdownEvent : UnityEvent<int> { }
        [Space(8)] public DropdownEvent dropdownEvent;

        // Audio
        public AudioClip hoverSound;
        public AudioClip clickSound;

        // Hidden variables
        [HideInInspector] public bool isOn;
        [HideInInspector] public int index = 0;
        [HideInInspector] public int siblingIndex = 0;
        [HideInInspector] public TextMeshProUGUI setItemText;
        [HideInInspector] public Image setItemImage;
        Sprite imageHelper;
        string textHelper;

        public enum AnimationType
        {
            FADING,
            SLIDING,
            STYLISH
        }

        [System.Serializable]
        public class Item
        {
            public string itemName = "Dropdown Item";
            public Sprite itemIcon;
            public UnityEvent OnItemSelection;
        }

        void Start()
        {
            Debug.Log("Entramos en el start del dropdown");
            try
            {
                dropdownAnimator = gameObject.GetComponent<Animator>();
                itemList = itemParent.GetComponent<VerticalLayoutGroup>();

                if (dropdownItems.Count != 0)
                {
                    SetupDropdown(true); //suponemos que es el de idiomas
                }
                else //si esta vacio suponemos que es el dropdown de cases
                {
                    //Debug.Log($"Vamos a hacer el setup de los Cases");
                    SetUpCases();
                }

                //Debug.Log($"No ha habido errores");
                currentListParent = transform.parent;
            }

            catch
            {
                Debug.LogError("Dropdown - Cannot initalize the object due to missing resources.", this);
            }

            if (enableScrollbar == true)
                itemList.padding.right = 25;

            else
                itemList.padding.right = 8;

            if (setHighPriorty == true)
                transform.SetAsLastSibling();

            if (saveSelected == true)
            {
                if (invokeAtStart == true)
                    dropdownItems[PlayerPrefs.GetInt(dropdownTag + "Dropdown")].OnItemSelection.Invoke();
                //else
                  //  ChangeDropdownInfo(PlayerPrefs.GetInt(dropdownTag + "Dropdown"));
            }
        }

        private void SetUpCases()
        {

            InitialLoad();
            dropdownItems.Add(new Item() { itemName = "Tutorial"});
            foreach (var item in cases)
            {
                Item aux = new Item();

                string[] parts;
                parts = item.name.Split("_"[0]);
                aux.itemName = parts[0].ToUpper();
                if (!Exists(aux.itemName))
                {
                    dropdownItems.Add(aux);

                    //Añadimos un item caseX_thirdPerson

                    Item aux2 = new Item();
                    if (aux.itemName == "Case3" || aux.itemName == "CASE3")
                    {
                        //aux2.itemName = aux.itemName + "_TP";//_ThirdPerson
                        //dropdownItems.Add(aux2);
                    }
                }
            }
            //Debug.Log($"Ahora vamos a hacer el setup del dropdown");
            SetupDropdown(false);
        }

        private bool Exists(string element)
        {
            foreach (var item in dropdownItems)
            {
                if (item.itemName == element)
                {
                    return true;
                }
            }
            return false;
        }

        public void InitialLoad()
        {
            var casesArray = Resources.LoadAll("Cases", typeof(DialogueContainer));
            //Debug.Log("Se han obtenido los siguientes casos:");
            foreach (var item in casesArray)
            {
                //Debug.Log($"{item.name}");
                cases.Add(item as DialogueContainer);
                //Debug.Log($"{cases[cases.Count-1].name}");
            }
        }

        public void SetupDropdown(bool isLanguage = false) 
        {
            foreach (Transform child in itemParent)
                GameObject.Destroy(child.gameObject);

            index = 0;
            for (int i = 0; i < dropdownItems.Count; ++i)
            {
                GameObject go = Instantiate(itemObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(itemParent, false);

                setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                textHelper = dropdownItems[i].itemName;
                setItemText.text = textHelper;

                Transform goImage;
                goImage = go.gameObject.transform.Find("Icon");
                setItemImage = goImage.GetComponent<Image>();
                imageHelper = dropdownItems[i].itemIcon;
                setItemImage.sprite = imageHelper;

                Button itemButton;
                itemButton = go.GetComponent<Button>();

                itemButton.onClick.AddListener(Animate);
                itemButton.onClick.AddListener(delegate
                {
                    ChangeDropdownInfo(index = go.transform.GetSiblingIndex());
                    dropdownEvent.Invoke(index = go.transform.GetSiblingIndex());

                    if (saveSelected == true)
                        PlayerPrefs.SetInt(dropdownTag + "Dropdown", go.transform.GetSiblingIndex());
                });

                if (dropdownItems[i].OnItemSelection != null)
                    itemButton.onClick.AddListener(dropdownItems[i].OnItemSelection.Invoke);

                if (invokeAtStart == true)
                    dropdownItems[i].OnItemSelection.Invoke();
            }

            //English seria el primero

            selectedText.text = dropdownItems[selectedItemIndex].itemName;
            selectedImage.sprite = dropdownItems[selectedItemIndex].itemIcon;
            currentListParent = transform.parent;

            Debug.Log("Vamos a comprobar si es el DROPDOWN de idiomas!");

            //Asignar aqui el selected.text en caso de ser un languague
            if (isLanguage)
            {
                UpdateLanguage(selectedText.text);
            }
        }

        public void UpdateLanguage(string language)
        {
            Debug.Log("Es el dropdown de idiomas");
            switch (language)
            {
                case "English":
                    Debug.Log("El idioma seleccionado es ingles");
                    LanguageManager.Instance.SelectLanguage("EN");
                    break;
                case "Español":
                    Debug.Log("El idioma seleccionado es español");
                    LanguageManager.Instance.SelectLanguage("ES");
                    break;
                case "Português":
                    Debug.Log("El idioma seleccionado es portugues");
                    LanguageManager.Instance.SelectLanguage("PT");
                    break;
                case "Čeština":
                    Debug.Log("El idioma seleccionado es checo");
                    LanguageManager.Instance.SelectLanguage("CZ");
                    break;
                case "Hungarian":
                    Debug.Log("El idioma seleccionado es hungaro");
                    LanguageManager.Instance.SelectLanguage("HU");
                    break;
            }
        }

        public void ChangeDropdownInfo(int itemIndex)
        {
            if (selectedImage != null)
                selectedImage.sprite = dropdownItems[itemIndex].itemIcon;

            if (selectedText != null)
                selectedText.text = dropdownItems[itemIndex].itemName;

            if (enableDropdownSounds == true && useClickSound == true)
                soundSource.PlayOneShot(clickSound);

            selectedItemIndex = itemIndex;
        }

        public void Animate()
        {
            if (isOn == false && animationType == AnimationType.FADING)
            {
                dropdownAnimator.Play("Fading In");
                isOn = true;

                if (isListItem == true)
                {
                    siblingIndex = transform.GetSiblingIndex();
                    gameObject.transform.SetParent(listParent, true);
                }
            }

            else if (isOn == true && animationType == AnimationType.FADING)
            {
                dropdownAnimator.Play("Fading Out");
                isOn = false;

                if (isListItem == true)
                {
                    gameObject.transform.SetParent(currentListParent, true);
                    gameObject.transform.SetSiblingIndex(siblingIndex);
                }
            }

            else if (isOn == false && animationType == AnimationType.SLIDING)
            {
                dropdownAnimator.Play("Sliding In");
                isOn = true;

                if (isListItem == true)
                {
                    siblingIndex = transform.GetSiblingIndex();
                    gameObject.transform.SetParent(listParent, true);
                }
            }

            else if (isOn == true && animationType == AnimationType.SLIDING)
            {
                dropdownAnimator.Play("Sliding Out");
                isOn = false;

                if (isListItem == true)
                {
                    gameObject.transform.SetParent(currentListParent, true);
                    gameObject.transform.SetSiblingIndex(siblingIndex);
                }
            }

            else if (isOn == false && animationType == AnimationType.STYLISH)
            {
                dropdownAnimator.Play("Stylish In");
                isOn = true;

                if (isListItem == true)
                {
                    siblingIndex = transform.GetSiblingIndex();
                    gameObject.transform.SetParent(listParent, true);
                }
            }

            else if (isOn == true && animationType == AnimationType.STYLISH)
            {
                dropdownAnimator.Play("Stylish Out");
                isOn = false;

                if (isListItem == true)
                {
                    gameObject.transform.SetParent(currentListParent, true);
                    gameObject.transform.SetSiblingIndex(siblingIndex);
                }
            }

            if (enableTrigger == true && isOn == false)
                triggerObject.SetActive(false);

            else if (enableTrigger == true && isOn == true)
                triggerObject.SetActive(true);

            if (outOnPointerExit == true)
                triggerObject.SetActive(false);

            if (setHighPriorty == true)
                transform.SetAsLastSibling();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (outOnPointerExit == true)
            {
                if (isOn == true)
                {
                    Animate();
                    isOn = false;
                }

                if (isListItem == true)
                    gameObject.transform.SetParent(currentListParent, true);
            }
        }

        public void UpdateValues()
        {
            if (enableScrollbar == true)
            {
                itemList.padding.right = 25;
                scrollbar.SetActive(true);
            }

            else
            {
                itemList.padding.right = 8;
                scrollbar.SetActive(false);
            }

            if (enableIcon == false)
                selectedImage.gameObject.SetActive(false);
            else
                selectedImage.gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enableDropdownSounds == true && useClickSound == true)
                soundSource.PlayOneShot(clickSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableDropdownSounds == true && useHoverSound == true)
                soundSource.PlayOneShot(hoverSound);
        }

        public void CreateNewItem(string title, Sprite icon)
        {
            Item item = new Item();
            item.itemName = title;
            item.itemIcon = icon;
            dropdownItems.Add(item);
            SetupDropdown();
        }

        public void AddNewItem()
        {
            Item item = new Item();
            dropdownItems.Add(item);
        }
    }
}