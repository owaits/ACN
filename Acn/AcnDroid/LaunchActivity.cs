#region Copyright © 2012 Mark Daniel
//______________________________________________________________________________________________________________
// AcnDroid
// Copyright © 2012 Mark Daniel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion

using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Acn.Helpers;
using System.Collections.Generic;

namespace AcnDroid
{
    /// <summary>
    /// Main Entry Point activity
    /// </summary>
    [Activity(Label = "AcnDroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class LaunchActivity : Activity
    {
        private SlpDeviceManager slpMangager;

        public SlpDeviceManager SlpManager
        {
            get { return slpMangager; }
            set
            {
                if (slpMangager != value)
                {
                    if (slpMangager != null)
                    {
                        slpMangager.UpdateComplete -= slpMangager_UpdateComplete;
                        slpMangager.Dispose();

                    }
                    slpMangager = value;
                    if (slpMangager != null)
                    {
                        slpMangager.UpdateComplete += slpMangager_UpdateComplete;
                    }
                }
            }
        }

        private ListView listView;
        private EditText scopeText;
        private EditText serviceTypeText;
        private List<string> devices = new List<string>();
        private ArrayAdapter<string> adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            SlpManager = new SlpDeviceManager();


            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            listView = FindViewById<ListView>(Resource.Id.listView1);
            scopeText = FindViewById<EditText>(Resource.Id.editTextScope);
            serviceTypeText = FindViewById<EditText>(Resource.Id.editTextServiceType);


            adapter = new ArrayAdapter<string>(this,
               Android.Resource.Layout.SimpleListItem1,
               devices);

            listView.Adapter = adapter;

            button.Click += delegate
            {
                if (SlpManager.Running)
                {
                    SlpManager.Stop();
                    button.Text = "Start";
                }
                else
                {
                    SlpManager.Scope = scopeText.Text;
                    SlpManager.ServiceType = serviceTypeText.Text;
                    SlpManager.Start();
                    button.Text = "Stop";
                }
            };
        }


        /// <summary>
        /// Handles the UpdateComplete event of the SLP Mangager.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SlpUpdateEventArgs" /> instance containing the event data.</param>
        void slpMangager_UpdateComplete(object sender, SlpUpdateEventArgs e)
        {

            RunOnUiThread(() =>
            {
                adapter.Clear();
                foreach (var description in SlpManager.GetDevices().
                    Select(d => string.Format("{0}\t{1}", d.Url, d.State)))
                {
                    adapter.Add(description);
                }
                adapter.NotifyDataSetChanged();
            });
        }

    }
}

