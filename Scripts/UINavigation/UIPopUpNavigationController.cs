using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace MVC.UISystem
{
    public interface IPopUp : IViewController
    {
        void Show();
        event EventHandler OnClose;
    }

    public sealed class UIPopUpNavigationController : System.Object
    {
        private static UIPopUpNavigationController _instance = null;
        public static UIPopUpNavigationController Instance
            => _instance ?? (_instance = new UIPopUpNavigationController());

        private Queue<IPopUp> popUpControllers = new Queue<IPopUp>();

        private UIPopUpNavigationController() { }

        public IPopUp VisiblePopUp
        {
            get
            {
                if (popUpControllers.Count > 0)
                    return popUpControllers.Peek();
                else
                    return null;
            }
        }

        public void AddToQueue(IPopUp popUp)
        {
            popUpControllers.Enqueue(popUp);
            popUp.OnClose += PopUp_OnClose;

            if (popUpControllers.Count == 1)
                MoveNext();
        }

        void PopUp_OnClose(object sender, EventArgs e)
        {
            IPopUp iSender = sender as IPopUp;
            iSender.OnClose -= PopUp_OnClose;

            var old = popUpControllers.Dequeue();
            Assert.AreEqual(iSender, old);

            MoveNext();
        }

        private void MoveNext() 
            => popUpControllers.Peek()?.Show();
    }
}
