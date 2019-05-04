using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.ContactService.Shared;

namespace Archetypical.Software.Trestle.Xamarin
{
    public static class TrestleHelper
    {
        public static IList<Contact> GetContacts()
        {
            var contacts = Plugin.ContactService.CrossContactService.Current.GetContactList();
            return contacts.ToList();
        }
    }
}
