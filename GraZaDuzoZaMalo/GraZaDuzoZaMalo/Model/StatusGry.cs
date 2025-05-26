using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Piotr Bacior 15 722 - WSEI Kraków

namespace GraZaDuzoZaMalo.Model
{
    //Definiujemy teraz typ wyliczeniowy enum o nazwie StatusGry, określający wszystkie możliwe statusy naszej gry
    public enum StatusGry
    {
        //Ustawiamy typ wyliczeniowy Trwa - czyli gra trwa, jest aktywna
        Trwa,

        //Ustawiamy typ wyliczeniowy Zawieszona - czyli gra jest zawieszona, nie jest aktywna (jest wstrzymana)
        Zawieszona,

        //Ustawiamy typ wyliczeniowy Zakonczona - czyli gra została zakończona, nie jest już aktywna
        Zakonczona
    }
}

