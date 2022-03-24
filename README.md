【MVC架構 - Entity 改用 ADO .NET 取得DB資料】
==================================
* 重構本機不允許使用 Entity framework開發，故改用ADO .NET取代，並延用MVC架構。
* View
  * 全域的資料存取，延用MVC原本寫法(ex. ~/XQLiteMgm/Views/Shared/_MenuPartial.cshtml)
  * 其它User操作的主要功能採用 React JS + Ant Design
