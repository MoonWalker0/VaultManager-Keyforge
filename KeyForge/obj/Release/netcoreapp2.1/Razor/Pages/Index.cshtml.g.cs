#pragma checksum "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0655e68c49ed16afbde62d0e05f5f2e2a136adb2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(KeyForge.Pages.Pages_Index), @"mvc.1.0.razor-page", @"/Pages/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure.RazorPageAttribute(@"/Pages/Index.cshtml", typeof(KeyForge.Pages.Pages_Index), null)]
namespace KeyForge.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#line 2 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\_ViewImports.cshtml"
using KeyForge;

#line default
#line hidden
#line 3 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\_ViewImports.cshtml"
using KeyForge.Data;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0655e68c49ed16afbde62d0e05f5f2e2a136adb2", @"/Pages/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3c1b6ffb86ea270d2bb2b4d89c42ed3f45ff9daf", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Index : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 3 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
  
    ViewData["Title"] = "Vault Manager";

#line default
#line hidden
            BeginContext(126, 6, true);
            WriteLiteral("\r\n<h2>");
            EndContext();
            BeginContext(133, 17, false);
#line 8 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(150, 9, true);
            WriteLiteral("</h2>\r\n\r\n");
            EndContext();
#line 10 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
 if (Model.Message != null)
{

#line default
#line hidden
            BeginContext(191, 34, true);
            WriteLiteral("    <div class=\"alert alert-info\">");
            EndContext();
            BeginContext(226, 13, false);
#line 12 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
                             Write(Model.Message);

#line default
#line hidden
            EndContext();
            BeginContext(239, 8, true);
            WriteLiteral("</div>\r\n");
            EndContext();
#line 13 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
}

#line default
#line hidden
            BeginContext(250, 420, true);
            WriteLiteral(@"
<h3>Manager turniejów Keyforge na bazie rankingu mELO (modyfikowany ELO)</h3>
<br /><br />

<h2> Ostatnie 10 wyników </h2>
<table class=""table"">
    <tr>
        <th> # </th>
        <th> Zwycięzca </th>
        <th> Pokonany </th>
        <th> Turniej </th>
        <th> + </th>
        <th> - </th>
        <th> ELO zwycięzcy </th>
        <th> ELO pokonanego </th>
        <th> Data </th>
    </tr>
");
            EndContext();
#line 31 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
       var i = 1; 

#line default
#line hidden
            BeginContext(691, 4, true);
            WriteLiteral("    ");
            EndContext();
#line 32 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
     foreach (var row in Model.History)
    {

#line default
#line hidden
            BeginContext(739, 31, true);
            WriteLiteral("        <tr>\r\n            <td> ");
            EndContext();
            BeginContext(771, 1, false);
#line 35 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(i);

#line default
#line hidden
            EndContext();
            BeginContext(772, 25, true);
            WriteLiteral(" </td>\r\n            <td> ");
            EndContext();
            BeginContext(798, 13, false);
#line 36 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(row.PlayerWin);

#line default
#line hidden
            EndContext();
            BeginContext(811, 25, true);
            WriteLiteral(" </td>\r\n            <td> ");
            EndContext();
            BeginContext(837, 14, false);
#line 37 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(row.PlayerLose);

#line default
#line hidden
            EndContext();
            BeginContext(851, 25, true);
            WriteLiteral(" </td>\r\n            <td> ");
            EndContext();
            BeginContext(877, 14, false);
#line 38 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(row.Tournament);

#line default
#line hidden
            EndContext();
            BeginContext(891, 25, true);
            WriteLiteral(" </td>\r\n            <td> ");
            EndContext();
            BeginContext(917, 15, false);
#line 39 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(row.WinIncrease);

#line default
#line hidden
            EndContext();
            BeginContext(932, 25, true);
            WriteLiteral(" </td>\r\n            <td> ");
            EndContext();
            BeginContext(958, 16, false);
#line 40 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(row.LoseDecrease);

#line default
#line hidden
            EndContext();
            BeginContext(974, 25, true);
            WriteLiteral(" </td>\r\n            <td> ");
            EndContext();
            BeginContext(1000, 15, false);
#line 41 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(row.WinFinalELO);

#line default
#line hidden
            EndContext();
            BeginContext(1015, 25, true);
            WriteLiteral(" </td>\r\n            <td> ");
            EndContext();
            BeginContext(1041, 16, false);
#line 42 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(row.LoseFinalELO);

#line default
#line hidden
            EndContext();
            BeginContext(1057, 25, true);
            WriteLiteral(" </td>\r\n            <td> ");
            EndContext();
            BeginContext(1083, 8, false);
#line 43 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
            Write(row.Date);

#line default
#line hidden
            EndContext();
            BeginContext(1091, 23, true);
            WriteLiteral(" </td>\r\n        </tr>\r\n");
            EndContext();
#line 45 "C:\Users\jedrz\Desktop\KeyForge\KeyForge\KeyForge\Pages\Index.cshtml"
        i++;
    }

#line default
#line hidden
            BeginContext(1135, 12, true);
            WriteLiteral("</table>\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public SignInManager<IdentityUser> SignInManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IndexModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<IndexModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<IndexModel>)PageContext?.ViewData;
        public IndexModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591