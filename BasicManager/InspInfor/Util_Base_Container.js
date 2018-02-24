var util = {
    //获取url中所传的参数
    getUrlParam: function (param) {
        //var params = Ext.urlDecode(location.search.substring(1));
        //return param ? params[param] : params;
        return "base_containerstandard";
    },
    //获取ext_onready中的panel：title
    getPanelTitle: function (param) {
        var title = "";
        switch (param) {
            case "base_containerstandard":
                title = "集装箱规格";
                break;
        }
        return title;
    },

    //获取init_search中的searchCode，searchName  //获取grindpanel中前两行
    getInitSearchSearchCode: function (param) {
        var title = "";
        switch (param) {
            case "base_containerstandard":
                title = "集装箱规格代码";
                break;
        }
        return title;
    },

    getInitSearchSearchName: function (param) {
        var title = "";
        switch (param) {
            case "base_containerstandard":
                title = "集装箱规格名称";
                break;
        }
        return title;
    },

    getGridNameCol3:function(param){
        var title ="";
        switch(param){
            case "base_containerstandard":
                title = "集装箱HS编码";
                break;
        }
        return title;
    },

    getGridNameCol4:function(param){
        var title ="";
        switch(param){
            case "base_containerstandard":
                title = "集装箱商品名称";
                break;
        }
        return title;
    },

    getGridNameCol5: function (param) {
        var title = "";
        switch (param) {
            case "base_containerstandard":
                title = "检验检疫类别";
                break;
        }
        return title;
    },

    getGridNameCol6: function (param) {
        var title = "";
        switch (param) {
            case "base_containerstandard":
                title = "海关监管条件";
                break;
        }
        return title;
    },


    //获取gridpanel中的colums
    getGrindPanelColumsOne: function (param) {
        var arr = new Array();
        switch (param) {
            case "base_containerstandard":
                arr[0] = "CODE";
                arr[1] = "NAME";
                arr[2] = "HSCODE";
                arr[3] = "HSNAME";
                arr[4] = "INSPECTION";
                arr[5] = "DECLARATION";
                break;

        }
        return arr;
    }
}