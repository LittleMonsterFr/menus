using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menus
{
    public class Type
    {
        public Type(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            var type = obj as Type;
            return type != null &&
                   Id == type.Id &&
                   Name.Equals(type.Name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public override string ToString()
        {
            return Name;
        }


    }
}
