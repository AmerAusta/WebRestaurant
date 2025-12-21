using BusinessLayer.Category;
using DataAccessLayer.Category;
using DataAccessLayer.MenuItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.MenuItems
{
    public class MenuItems
    {
        public enum enMode { AddNew = 0, Update = 1 };
        enMode Mode = enMode.AddNew;
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PerparationTime { get; set; }
        public bool IsActive { get; set; }

        public MenuItemDTO MDTO
        {
            get{return new MenuItemDTO (MenuItemId, Name, Description,CategoryId,Price,CreatedAt,PerparationTime,IsActive);}
        }

        public MenuItems(MenuItemDTO menuItemDTO,enMode mMode=enMode.AddNew)
        {
            MenuItemId = menuItemDTO.MenuItemId;
            Name = menuItemDTO.Name;
            Description = menuItemDTO.Description;
            CategoryId = menuItemDTO.CategoryId;
            Price = menuItemDTO.Price;
            CreatedAt = menuItemDTO.CreatedAt;
            PerparationTime = menuItemDTO.PerparationTime;
            IsActive=menuItemDTO.IActive;
            Mode= mMode;
        }

        public static MenuItems Find(int MenuItemID)
        {
            MenuItemDTO MDTO=MenuItemsData.GetMenuItemByMenuItemId(MenuItemID);
            if(MDTO!=null)
            {
                return new MenuItems(MDTO,enMode.Update);
            }
            else
            {
                return null;
            }
        }

        public static List<MenuItems> GetAllMenuItems()
        {
            List<MenuItemDTO> listDTO = MenuItemsData.GetAllMenuItems();

            if (listDTO == null || listDTO.Count == 0)
                return null;

            List<MenuItems> list = new List<MenuItems>();

            foreach (var dto in listDTO)
            {
                list.Add(new MenuItems(dto));
            }

            return list;
        }

        public static List<MenuItems> GetAllMenuItemsByCategoryID(int CategoryID)
        {
            List<MenuItemDTO> listDTO = MenuItemsData.GetAllMenuItemsByCategoryID(CategoryID);

            if (listDTO == null || listDTO.Count == 0)
                return null;

            List<MenuItems> list = new List<MenuItems>();

            foreach (var dto in listDTO)
            {
                list.Add(new MenuItems(dto));
            }

            return list;
        }

        private bool _AddNewMenuItem()
        {

            MenuItemId = MenuItemsData.AddNewMenuItem(MDTO);
            return MenuItemId != -1;
        }

        private bool _UpdateMenuItem()
        {
            return MenuItemsData.UpdateMenuItem(MDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewMenuItem())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                case enMode.Update:
                    return _UpdateMenuItem();
            }
            return false;
        }

        public static bool DeleteMenuItem(int MenuItemID)
        {
            return MenuItemsData.DeleteMenuItem(MenuItemID);
        }
    }
}
