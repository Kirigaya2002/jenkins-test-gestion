using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace PasswordValidatorTests
{
    [TestClass]
    public class PasswordValidatorTests
    {
        private IPasswordValidator? _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new PasswordValidator();
        }

        [TestMethod]
        public void Password_Valid_ReturnsTrue()
        {
            Assert.IsNotNull(_validator);
            Assert.IsTrue(_validator.IsValid("Abcd1234!"));
        }

        [TestMethod]
        public void Password_LessThan8Characters_ReturnsFalse()
        {
            Assert.IsNotNull(_validator);
            Assert.IsFalse(_validator.IsValid("Ab1!"));
        }

        [TestMethod]
        public void Password_NoUpperCase_ReturnsFalse()
        {
            Assert.IsNotNull(_validator);
            Assert.IsFalse(_validator.IsValid("abcd1234!"));
        }

        [TestMethod]
        public void Password_NoLowerCase_ReturnsFalse()
        {
            Assert.IsNotNull(_validator);
            Assert.IsFalse(_validator.IsValid("ABCD1234!"));
        }

        [TestMethod]
        public void Password_NoNumber_ReturnsFalse()
        {
            Assert.IsNotNull(_validator);
            Assert.IsFalse(_validator.IsValid("Abcdefg!"));
        }

        [TestMethod]
        public void Password_NoSpecialCharacter_ReturnsFalse()
        {
            Assert.IsNotNull(_validator);
            Assert.IsFalse(_validator.IsValid("Abcd1234"));
        }

        [TestMethod]
        public void Password_Null_ReturnsFalse()
        {
            Assert.IsNotNull(_validator);
            Assert.IsFalse(_validator.IsValid(null));
        }

        [TestMethod]
        public void Password_EmptyString_ReturnsFalse()
        {
            Assert.IsNotNull(_validator);
            Assert.IsFalse(_validator.IsValid(""));
        }

        [TestMethod]
        public void Password_OnlySpaces_ReturnsFalse()
        {
            Assert.IsNotNull(_validator);
            Assert.IsFalse(_validator.IsValid("        "));
        }
    }
}
