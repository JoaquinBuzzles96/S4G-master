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
    public string m_UserName = "";
    public string m_UserMail = "";

   public TextMeshProUGUI m_Name;
   public TextMeshProUGUI m_Mail;


    public void SendEmail()
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("serroddev@gmail.com");
        mail.To.Add(m_UserMail);
        mail.Subject = "Usuario y Correo";

        mail.Body = "Name: " + m_UserName + " Correo: " + m_UserMail;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("serroddev@gmail.com", "sergikovic63") as ICredentialsByHost;
        smtpServer.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { return true; };
        smtpServer.Send(mail);
    }


    public void SaveName(Keyboard l_Field)
    {
        m_UserName=l_Field.text;
        m_Name.text = m_UserName;
    } 
    
    public void SaveMail(Keyboard l_Field)
    {
        m_UserMail = l_Field.text;
        m_Mail.text = m_UserMail;
    }
    
    public void OpenKeyboardName(Keyboard l_Keyboard)
    {
       // m_UserName=l_Field.text;
    } 
    
    public void OpenKeyboardMail(Keyboard l_Keyboard)
    {
       // m_UserMail = l_Field.text;
    }
}
