using BusinessLayer.Helper;
using DataAccessLayer.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Category
{
    public class Category
    {
        public enum enMode { AddNew = 0, Update = 1 };
        enMode Mode = enMode.AddNew;
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public CategoryDTO CDTO
        { get { return new CategoryDTO(CategoryID, Name, Description); } }

        public Category(CategoryDTO categoryDTO, enMode cMode=enMode.AddNew) 
        {
            CategoryID = categoryDTO.CategoryID;
            Name = categoryDTO.Name;
            Description = categoryDTO.Description;
            Mode = cMode;
        }

        public static Category Find(int CategoryID)
        {
            CategoryDTO CDTO = CategoryData.GetCategoryByCategoryID(CategoryID);

            if (CDTO != null)
            {
                return new Category(CDTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }

        public static List<Category> GetAllCategory()
        {
            List<CategoryDTO> listDTO = CategoryData.GetAllCategory();

            if (listDTO == null || listDTO.Count == 0)
                return null;

            List<Category> list = new List<Category>();

            foreach (var dto in listDTO)
            {
                list.Add(new Category(dto));
            }

            return list;
        }

        private bool _AddNewCategory()
        {

            CategoryID = CategoryData.AddNewCategory(CDTO);
            return CategoryID != -1;
        }

        private bool _UpdateCategory()
        {
            return CategoryData.UpdateCategory(CDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewCategory())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                case enMode.Update:
                    return _UpdateCategory();
            }
            return false;
        }

        public static bool DeleteCategory(int CategoryID)
        {
            return CategoryData.DeleteCategory(CategoryID);
        }
    }
}
