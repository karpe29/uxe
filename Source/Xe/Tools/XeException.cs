using System;
using System.Collections.Generic;
using System.Text;
#if !XBOX360
using System.Windows.Forms;

using System.Net;
using System.Net.Mail;
#endif
using Microsoft.SqlServer.MessageBox;

namespace XeFramework
{
		public class XeExceptionTool
		{
			public static void ShowExceptionMessageBox(System.Exception e)
			{
				ExceptionMessageBox emb =
						new ExceptionMessageBox(e,
						ExceptionMessageBoxButtons.Custom,
						ExceptionMessageBoxSymbol.Error,
						ExceptionMessageBoxDefaultButton.Button2,
						ExceptionMessageBoxOptions.None);

				emb.SetButtonText("Send By Mail", "Retry", "Continue", "Quit");

				emb.Caption = "A " + e.GetType().ToString() + " has occurred";
				emb.ShowToolBar = true;

				emb.HelpLink = "http://xe3d.free.fr/";

				emb.Show(null);

				switch (emb.CustomDialogResult)
				{
					case ExceptionMessageBoxDialogResult.Button1: // send by mail
						MailMessage thisMail = new MailMessage();
						thisMail.From = new MailAddress("jbriguet@free.fr");
						thisMail.To.Add(new MailAddress("jbriguet@free.fr"));
						thisMail.Subject = "Xe3D Automatic Bug Report for " + e.GetType().ToString();
						thisMail.IsBodyHtml = false;
						thisMail.Body = "Hi,\nFind the bug log in attachments.";
						
/*						
						Attachment thisAttachment = new Attachment("Xe3D.log");
						thisMail.Attachments.Add(thisAttachment);

						CredentialCache thisCredentialsCache = new CredentialCache();
						SmtpClient thisSmtp = new SmtpClient("smtp.domainepublic.net");
						thisSmtp.Send(thisMail);
*/						 
						MessageBox.Show("Not Totally Implemented yet :)", "Ooops");
						break;

					case ExceptionMessageBoxDialogResult.Button2: // retry 
						throw e;

					case ExceptionMessageBoxDialogResult.Button3: // continue
						break;

					case ExceptionMessageBoxDialogResult.Button4:
						Application.Exit();
						break;
				}
			}

			public static void ThreatException(System.Exception e)
			{
				try
				{
					ShowExceptionMessageBox(e);
				}
				catch (System.Exception)
				{
				}
			}
		}
}
