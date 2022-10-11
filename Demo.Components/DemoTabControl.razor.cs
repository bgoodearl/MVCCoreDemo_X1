using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Demo.Components
{
    #region License
    /*
    MIT License

    Copyright(c) 2019 Peter Morris

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    */
    #endregion License

    public partial class DemoTabControl
    {
        // Next line is needed so we are able to add <TabPage> components inside
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public DemoTabPage ActivePage { get; set; }
        List<DemoTabPage> Pages = new List<DemoTabPage>();

        internal void AddPage(DemoTabPage tabPage)
        {
            Pages.Add(tabPage);
            if (Pages.Count == 1)
                ActivePage = tabPage;
            StateHasChanged();
        }

        string GetButtonClass(DemoTabPage page)
        {
            return page == ActivePage ? "btn-primary" : "btn-secondary";
        }

        void ActivatePage(DemoTabPage page)
        {
            ActivePage = page;
        }
    }
}
