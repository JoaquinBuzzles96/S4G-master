﻿/*  ------ S4Game Consortium --------------
*  This library is free software; you can redistribute it and/or
*  modify it under the terms of the GNU Lesser General Public
*  License as published by the Free Software Foundation; either
*  version 3 of the License, or (at your option) any later version.
*  This library is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
*  Lesser General Public License for more details.
*  You should have received a copy of the GNU Lesser General Public
*  License along with this library; if not, write to the Free Software
*  CCMIJU, Carretera Nacional 521, Km 41.8 – 1007, Cáceres, Spain*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using UnityEngine.UI;
using TMPro;
using VRKeys;

public class SendMail : MonoBehaviour
{
    private static SendMail instance = null;

    // Game Instance Singleton
    public static SendMail Instance
    {
        get
        {
            return instance;
        }
    }

    [System.NonSerialized]
    public string m_UserName = "Default User"; //default value
    [System.NonSerialized]
    public string m_UserMail = "s4game@viralstudios.es"; //default value

    public TextMeshProUGUI m_NameMail;
    public TextMeshProUGUI m_Mail;

    public GameObject inputFieldName;
    public GameObject inputFieldMail;
    public GameObject imageMail;
    public GameObject nameMail;
    public GameObject textMail;
    public GameObject imageName;
    public GameObject nameName;
    public GameObject textName;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
#if UNITY_EDITOR_WIN || UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE
        if (inputFieldMail != null)
        {
            inputFieldMail.SetActive(true);
            inputFieldName.SetActive(true);
            imageMail.SetActive(false);
            textMail.SetActive(false);
            nameMail.SetActive(false);
            imageName.SetActive(false);
            nameName.SetActive(false);
            textName.SetActive(false);
        }
#endif
    }

    public void SendEmail_old()
    {

        string version = "Desktop";

#if UNITY_ANDROID //PLATFORM_ANDROID
            version = "VR";
#endif

        string text =
            " --------------- VERSION " + version + "(" + System.DateTime.Now + ") ---------------" +
            " Name: " + m_UserName + 
            "\n Correo: " + m_UserMail +
            "\n Case: " + LanguageManager.Instance.caseSelected + 
            "\n Languague: " + LanguageManager.Instance.languageSelected + 
            "\n Total time: " + Mathf.RoundToInt(Time.time / 60) + " minuts and " + Mathf.RoundToInt(Time.time % 60) + " seconds." + 
            "\n Total score:" + UI_Manager.Instance.totalScore + 
            "\n Total decisions:" + UI_Manager.Instance.totalDecisions + 
            "\n Correct decisions:" + UI_Manager.Instance.totalCorrectAnswers +
            UI_Manager.Instance.playereRoute;        

        //A parte de enviar el email, vamos a almacenar la informacion en un fichero de salida
        SaveCSV(text);

        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("S4GameViralStudios@gmail.com");
        mail.To.Add(m_UserMail);
        mail.Subject = "S4G TEST";
        /*
        Attachment attachment = new Attachment(@"D:\S4Game\somefile.txt");
        mail.Attachments.Add(attachment);
        */
        /*
        if (UI_Manager.Instance.playereRoute == null || UI_Manager.Instance.playereRoute == "")
        {
            UI_Manager.Instance.playereRoute = "\n Testing";
        }
        */

        mail.Body = text;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");//mail.viralstudios.es//"smtp.gmail.com"
        smtpServer.Port = 587;
        //smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;//testing
        //("s4game@viralstudios.es", "[l,=6?U=V,Cd")
        smtpServer.Credentials = new System.Net.NetworkCredential("S4GameViralStudios@gmail.com", " ViralStudios0@") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        /*
        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { return true; };
        */
        Debug.Log($"Enviamos el mail con el feedback al correo {m_UserMail}");
        smtpServer.Send(mail);
    }

    public void SendEmail()
    {
        string version = "Desktop";

#if UNITY_ANDROID //PLATFORM_ANDROID
        version = "VR";
#endif

        string text =
            " --------------- VERSION " + version + "(" + System.DateTime.Now + ") ---------------" +
            " Name: " + m_UserName +
            "\n Correo: " + m_UserMail +
            "\n Case: " + LanguageManager.Instance.caseSelected +
            "\n Languague: " + LanguageManager.Instance.languageSelected +
            "\n Total time: " + Mathf.RoundToInt(Time.time / 60) + " minuts and " + Mathf.RoundToInt(Time.time % 60) + " seconds." +
            "\n Total score:" + UI_Manager.Instance.scorePercentage + " %"+
            "\n Total decisions:" + UI_Manager.Instance.totalDecisions +
            "\n Correct decisions:" + UI_Manager.Instance.totalCorrectAnswers +
            UI_Manager.Instance.playereRoute;

        SaveCSV(text);

        MailMessage mail = new MailMessage();
        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
        mail.From = new MailAddress("S4GameViralStudios@gmail.com");
        mail.To.Add(m_UserMail);
        mail.Subject = "S4G TEST";
        mail.Body = text;

        SmtpServer.Port = 587;
        SmtpServer.Credentials = new System.Net.NetworkCredential("S4GameViralStudios@gmail.com", " ViralStudios0@");
        SmtpServer.EnableSsl = true;

        Debug.Log($"Enviamos el mail con el feedback al correo {m_UserMail}");

        SmtpServer.Send(mail);
    }

    public void SendEmail_test_working()
    {
        MailMessage mail = new MailMessage();
        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
        mail.From = new MailAddress("S4GameViralStudios@gmail.com");
        mail.To.Add(m_UserMail);
        mail.Subject = "Test Mail";
        mail.Body = "S4G: This is for testing SMTP mail from GMAIL";
        
        SmtpServer.Port = 587;
        SmtpServer.Credentials = new System.Net.NetworkCredential("S4GameViralStudios@gmail.com", " ViralStudios0@");
        SmtpServer.EnableSsl = true;

        Debug.Log($"Enviamos el mail con el feedback al correo {m_UserMail}");

        SmtpServer.Send(mail);

    }

    public void SaveCSV(string text)
    {
        

        //string path = "Assets/Resources/ficheroPrueba.txt";
        string path = Application.persistentDataPath + "/S4GDataFile.txt";

        Debug.Log($"VAMOS A GUARDAR EL CSV en ({path})");

        TextWriter tw = new StreamWriter(path, true);
        //DateTime.Now
        // Write a line of text to the file
        //tw.WriteLine($"-------------------- {System.DateTime.Now} --------------------");//Ejemplo {System.DateTime.Now}



        tw.WriteLine(text);

        // Close the stream
        tw.Close();

        /*
        //DEBUG
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();

        */
        /*
         * Add text at the end: File.AppendAllText("date.txt", DateTime.Now.ToString());
         * Add a new line: File.AppendAllText("date.txt", DateTime.Now.ToString() + Environment.NewLine);
         * Open it to edit(in theory): TextWriter tw = new StreamWriter("date.txt", true);
         */


    }

    public void testEmail()
    {
        Debug.Log($"Enviamos el mail con el feedback al correo {m_UserMail}, nombre: {m_UserName}");
    }

    public void SaveName(Keyboard l_Field)
    {
        m_UserName = l_Field.text;
        m_NameMail.text = m_UserName;
    }

    public void SaveMail(Keyboard l_Field)
    {
        m_UserMail = l_Field.text;
        Debug.Log($"Acabamos de guardar el mail {l_Field.text}, el valor de m_Usermail es: {m_UserMail}");
        m_Mail.text = m_UserMail;
    }

    public void SaveNamePC(TextMeshProUGUI l_Field)
    {
        m_UserName = l_Field.text;
        //m_NameMail.text = m_UserName;
    }

    public void SaveMailPC(TextMeshProUGUI l_Field)
    {
        m_UserMail = l_Field.text;
        Debug.Log($"Acabamos de guardar el mail {l_Field.text}, el valor de m_Usermail es: {m_UserMail}");
        // m_Mail.text = m_UserMail;
    }


}
