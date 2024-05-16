using CodeHelperClone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace CodeHelperClone
{
    public class WebRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }
        
        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-J3AFDKS;Initial Catalog=codeHelperClone;Integrated Security=True;Encrypt=False"))
            {
                SqlCommand cmd = new SqlCommand("select Role from tbl_RolesTable where USerID='" + username + "'", con);
                con.Open();
                SqlDataReader read  = cmd.ExecuteReader();
                while (read.Read())
                {
                    roles.Add(read["Role"].ToString());
                }
                con.Close();
            }
            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}