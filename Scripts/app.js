(function (document) {
    "use strict";

    $(document).ready(function () {
        $("#copyrightyear").html(new Date().getFullYear());

        //showBreadcrumb();

        $("table.gridview>tbody>tr>td").each(function () {
            if (this.offsetWidth < this.scrollWidth) {
                $(this).attr('data-title', $(this).text().trim());
            }
        });

        $(".alert").hover(alertHover);
        $(".alert").mouseleave(alertMouseleave);

        //$("select").select2();

    });

    //$(window).scroll(function () {
    //    $("#main h1").toggleClass('mainSectionTitleFixedOnScroll', $(window).scrollTop() > 36);
    //    $(".tabbable").toggleClass('mainSectionContentPositionOnScroll', $(window).scrollTop() > 36);
    //});

    //window.showBreadcrumb = function () {
    //    var currentPage = window.location.pathname.substring(1);
    //    var $initialLink = $('nav#menu a[href="' + currentPage + '"]');
    //    var $breadcrumb = $("section#navbar span").eq(0);
    //    $initialLink.addClass("active");
    //    var $origin = $initialLink.closest('li');

    //    if ($origin.length) {
    //        while ($origin.length) {
    //            $breadcrumb.prepend($origin.find('a')[0].outerHTML);
    //            $breadcrumb.prepend(' &raquo; ');
    //            $origin = $origin.parent().closest('li');
    //        }
    //    }
    //    $breadcrumb.prepend("<a href='Index.aspx'>Tela inicial</a>");
    //}

    window.enableTab = function (selector) {
        var $t = $(selector);
        var $a = $(selector + ' a');
        $(selector).removeClass('disabled'); 
        $(selector + ' a').attr('data-toggle', 'tab'); 
        $(selector + ' a').click();
    }

    window.alert = function (s, type) {
        var $div = $("<div></div>");
        $div.addClass("alert");
        switch (type) {
            case "success":
                $div.append("<span class='glyphicon glyphicon-ok-sign' aria-hidden='true'></span>");
                $div.addClass("alert-success");
                break;
            case "info":
                $div.append("<span class='glyphicon glyphicon-info-sign' aria-hidden='true'></span>");
                $div.addClass("alert-info");
                break;
            case "warning":
                $div.append("<span class='glyphicon glyphicon-exclamation-sign' aria-hidden='true'></span>");
                $div.addClass("alert-warning");
                break;
            case "danger":
                $div.append("<span class='glyphicon glyphicon-remove-sign' aria-hidden='true'></span>");
                $div.addClass("alert-danger");
                break;
            default:
                $div.append("<span class='glyphicon glyphicon-exclamation-sign' aria-hidden='true'></span>");
                $div.addClass("alert-warning");
                break;
        }
        $div.append(s);
        $div.delay(5000).fadeOut(5000);
        $div.hover(alertHover);
        $div.mouseleave(alertMouseleave);
        $('body').append($div);
    };

    window.alertHover = function () {
        $(this).clearQueue().stop().fadeIn(0);
        $(this).removeAttr("style");
        observer.disconnect();
    }

    window.alertMouseleave = function () {
        $(this).fadeOut(5000);
    }

    window.fillMessageContent = function (content) {
        $(document).ready(function () {
            $("header span.glyphicon-bell").popover({
                animation: true,
                placement: 'bottom',
                trigger: 'hover',
                html: true,
                title: 'Últimas mensagens do servidor',
                content: '<ul>' + content + '</ul>',
                container: 'body',
                viewport: 'section#main'

            }).on("show.bs.popover", function (e) {
                $(this).data()["bs.popover"].$tip.css("max-width", "600px");
            }).click(function () {
                $(this).removeClass("bell-ring");
            });

            $("header span.glyphicon-bell").addClass("bell-ring");
        });
    }

    window.fillConnectionInfo = function (content) {
        $(document).ready(function () {
            $("header span.glyphicon-hdd").popover({
                animation: true,
                placement: 'bottom',
                trigger: 'hover',
                html: true,
                title: 'Conexão com a base de dados',
                content: content,
                container: 'body',
                viewport: 'section#main'

            }).on("show.bs.popover", function (e) {
                $(this).data()["bs.popover"].$tip.css("max-width", "600px");
            });

        });
    }

})(this);

MutationObserver = window.MutationObserver || window.WebKitMutationObserver;

var observer = new MutationObserver(function (mutations, observer) {
    mutations.forEach(function (o) {
        if (o.target.className.includes('alert')) {
            $('.alert').delay(5000).fadeOut(5000);
        }
    });
});

observer.observe(document, {
    subtree: true,
    attributes: true
});
/*
childList, Set to true if mutations to target's children are to be observed.
attributes, Set to true if mutations to target's attributes are to be observed.
characterData, Set to true if mutations to target's data are to be observed.
subtree, Set to true if mutations to not just target, but also target's descendants are to be observed.
attributeOldValue, Set to true if attributes is set to true and target's attribute value before the mutation needs to be recorded.
characterDataOldValue, Set to true if characterData is set to true and target's data before the mutation needs to be recorded.
attributeFilter, Set to a list of attribute local names (without namespace) if not all attribute mutations need to be observed.
*/

var Base64 = {

    // private property
    _keyStr: "mn9+abxP1/RSTUKLMNefgh2Jijo4XYOQZ8klyzA0pqr67GVst5Wu=cd3HIBCDEFvw",

    // public method for encoding
    encode: function (input) {
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);

        while (i < input.length) {

            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output +
            this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
            this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);

        }

        return output;
    },

    // private method for UTF-8 encoding
    _utf8_encode: function (string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }

        return utftext;
    }

}