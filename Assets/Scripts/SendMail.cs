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
    public string m_UserName = "Joaquin"; //default value
    [System.NonSerialized]
    public string m_UserMail = "joakilm2@gmail.com"; //default value

   public TextMeshProUGUI m_NameMail;
   public TextMeshProUGUI m_Mail;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SendEmail()
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("s4game@viralstudios.es");
        mail.To.Add(m_UserMail);
        mail.Subject = "S4G TEST";
        /*
        Attachment attachment = new Attachment(@"D:\S4Game\somefile.txt");
        mail.Attachments.Add(attachment);
        */
        mail.Body = "Name: " + m_UserName + " Correo: " + m_UserMail + UI_Manager.Instance.playereRoute;//"probando TT";

        SmtpClient smtpServer = new SmtpClient("mail.viralstudios.es");//mail.viralstudios.es//"smtp.gmail.com"
        smtpServer.Port = 587;
        smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;//testing
        //("s4game@viralstudios.es", "[l,=6?U=V,Cd")
        //("joakilm2@gmail.com", "Quinotec96@")
        smtpServer.Credentials = new System.Net.NetworkCredential("s4game@viralstudios.es", "[l,=6?U=V,Cd") as ICredentialsByHost;
        smtpServer.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { return true; };
        Debug.Log($"Enviamos el mail con el feedback al correo {m_UserMail}");
        smtpServer.Send(mail);
    }


    public void SaveName(Keyboard l_Field)
    {
        m_UserName=l_Field.text;
        m_NameMail.text = m_UserName;
    } 
    
    public void SaveMail(Keyboard l_Field)
    {
        m_UserMail = l_Field.text;
        m_Mail.text = m_UserMail;
    }
    
   
}
