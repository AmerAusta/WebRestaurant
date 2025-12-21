using System;
using System.Data;
using BusinessLayer.Helper;
using DataAccessLayer.User;

namespace BusinessLayer.User
{
    public class Users
    {
        public enum enMode { AddNew=0, Update=1 };
        enMode Mode = enMode.AddNew;

        public int UserID {get; set;}
        public string FullName {get; set;}
        public string Email {get; set;}
        public string PasswordHash {get; set;}
        public UserDTO.enRoles RoleId { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDTO UDTO 
        { 
            get { return new UserDTO(UserID,FullName,Email,PasswordHash,RoleId,Phone,IsActive,CreatedAt); } 
        }

        public UserDTOresponse UDTOresponse
        {
            get { return new UserDTOresponse(UserID, FullName, Email, RoleId, Phone, IsActive); }
        }

        public Users(UserDTO UDTO,enMode uMode=enMode.AddNew)
        {
            UserID=UDTO.UserID;
            FullName=UDTO.FullName;
            Email=UDTO.Email;
            PasswordHash=UDTO.PasswordHash;
            RoleId=UDTO.RoleId;
            Phone=UDTO.Phone;
            IsActive=UDTO.IsActive;
            CreatedAt=UDTO.CreatedAt;

            Mode=uMode;
        }

        public static Users Find(int UserID)
        {
            UserDTO UDTO = UsersData.GetUserByID(UserID);

            if (UDTO != null)
            {
                return new Users(UDTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }

        public static Users GetUserByFullName(string FullName)
        {
            UserDTO UDTO = UsersData.GetUserByFullName(FullName);

            if (UDTO != null)
            {
                return new Users(UDTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }

        public static Users GetUserByEmailAndPassword(string Email, string Password)
        {
            UserDTO UDTO=UsersData.GetUserByEmail(Email);

            if(UDTO!=null)
            {
                if (PasswordHelper.VerifyPassword(Password,UDTO.PasswordHash))
                    return new Users(UDTO, enMode.Update);
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        private bool _AddNewUser()
        {
            PasswordHash=PasswordHelper.HashPassword(PasswordHash);
            UserID = UsersData.AddNewUser(UDTO);
            return UserID != -1;
        }

        private bool _UpdateUser()
        {
            if (!PasswordHelper.IsSha256Base64Hash(PasswordHash))
            {
                PasswordHash = PasswordHelper.HashPassword(PasswordHash);
            }
            return UsersData.UpdateUser(UDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                case enMode.Update:
                    return _UpdateUser();
            }
            return false;
        }

        public static bool DeleteUser(int UserID)
        {
            return UsersData.DeleteUser(UserID);
        }
    }
}
