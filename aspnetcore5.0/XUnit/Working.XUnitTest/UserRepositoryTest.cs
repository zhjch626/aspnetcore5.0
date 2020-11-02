using Moq;
using System;
using System.Data;
using Working.Models.Repository;
using Xunit;
using Dapper;
using Working.Models.DataModel;
using System.Collections.Generic;

namespace Working.XUnitTest
{
    /// <summary>
    /// �û��ִ�����
    /// </summary>
    [Trait("�û��ִ���", "UserRepository")]
    public class UserRepositoryTest
    {
        /// <summary>
        /// ���ݿ�Mock����
        /// </summary>
        Mock<IWorkingDB> _dbMock;
        /// <summary>
        /// �û��ִ�����
        /// </summary>
        IUserRepository _userRepository;
        public UserRepositoryTest()
        {
            _dbMock = new Mock<IWorkingDB>();
            _userRepository = new UserRepository(_dbMock.Object);
        }

        #region ��¼����
        /// <summary>
        /// ���Ե�¼����ֵ
        /// </summary>
        [Fact]
        public void Login_Default_Return()
        {
            var list = new List<UserRole>() {
                new UserRole{ ID=1, Name="����ΰ", DepartmentID=1, Password="gsw", RoleID=1, RoleName="Leader", UserName="gsw" }
            };
            _dbMock.Setup(db => db.Query<UserRole>(It.IsAny<string>(), It.IsAny<object>(), null, true, null, null)).Returns(list);
            var userRole = _userRepository.Login("gsw", "gsw");

            Assert.NotNull(userRole);


        }
        /// <summary>
        /// ���Ե�¼�û������������
        /// </summary>
        [Fact]
        public void Login_Null_ThrowException()
        {
            var list = new List<UserRole>();
            _dbMock.Setup(db => db.Query<UserRole>(It.IsAny<string>(), null, null, true, null, null)).Returns(list);
            var exc = Assert.Throws<Exception>(() => { _userRepository.Login("gsw", "gsw"); });

            Assert.Contains("�û������������", exc.Message);
        }
        /// <summary>
        /// ���Ե�¼�û������������
        /// </summary>
        [Fact]
        public void Login_Unkonow_ThrowException()
        {
            var list = new List<UserRole>();
            _dbMock.Setup(db => db.Query<UserRole>(It.IsAny<string>(), It.IsAny<object>(), null, true, null, null)).Throws(new Exception("δ֪"));
            var exc = Assert.Throws<Exception>(() => { _userRepository.Login("gsw", "gsw"); });
            Assert.Contains("δ֪", exc.Message);
        }
        #endregion

        #region �����û�����
        /// <summary>
        /// �����쳣����
        /// </summary>
        [Fact]
        public void AddUser_NullUser_ThrowException()
        {
            var exception = Assert.Throws<Exception>(() => { _userRepository.AddUser(null); });
            Assert.Contains("���ӵ��û�����ΪNull", exception.Message);
        }

        /// <summary>
        /// �����쳣����
        /// </summary>
        [Fact]
        public void AddUser_Default_ReturnTrue()
        {
            _dbMock.Setup(db => db.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(1);
            var result = _userRepository.AddUser(new User { UserName = "test" });
            Assert.True(result);
        }
        #endregion

        #region �޸��û�����
        /// <summary>
        /// �����쳣�޸�
        /// </summary>
        [Fact]
        public void ModifyUser_NullUser_ThrowException()
        {
            var exception = Assert.Throws<Exception>(() => { _userRepository.ModifyUser(null); });
            Assert.Contains("�޸ĵ��û�����ΪNull", exception.Message);
        }

        /// <summary>
        /// �����쳣�޸�
        /// </summary>
        [Fact]
        public void ModifyUser_Default_ReturnTrue()
        {
            _dbMock.Setup(db => db.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(1);
            var result = _userRepository.ModifyUser(new User { UserName = "test" });
            Assert.True(result);
        }
        #endregion

        #region �޸��û����� ,�޸��������    

        /// <summary>
        /// �����û��޸�
        /// </summary>
        /// <param name="userID">�û�ID</param>
        /// <param name="backResult">����ֵ</param>
        [Theory]
        [InlineData(1, 1)]
        [InlineData(0, 0)]
        public void RemoveUser_Default_ReturnTrue(int userID, int backResult)
        {
            _dbMock.Setup(db => db.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(backResult);
            var result = _userRepository.RemoveUser(userID);
            Assert.Equal(userID == 1, result);
        }


        /// <summary>
        /// �޸��û�������ȷ����
        /// </summary>
        [Fact]
        public void ModifyPassword_Default_ReturnTrue()
        {
            var list = new List<User>() { new User { ID = 1, Name = "����ΰ", DepartmentID = 1, Password = "gsw", RoleID = 1, UserName = "gsw" } };
            _dbMock.Setup(db => db.Query<User>(It.IsAny<string>(), It.IsAny<object>(), null, true, null, null)).Returns(list);

            _dbMock.Setup(db => db.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(1);
            var result = _userRepository.ModifyPassword("ggg", "gsw", 1);
            Assert.True(result);
        }
        /// <summary>
        /// �޸��û������쳣����
        /// </summary>
        [Fact]
        public void ModifyPassword_NullUser_ThrowException()
        {
            var list = new List<User>();
            _dbMock.Setup(db => db.Query<User>(It.IsAny<string>(), It.IsAny<object>(), null, true, null, null)).Returns(list);
            _dbMock.Setup(db => db.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(1);
            var exc = Assert.Throws<Exception>(() => _userRepository.ModifyPassword("ggg", "gsw", 1));
            Assert.Contains("�޸�����:�޸�����ʧ��:�����벻��ȷ", exc.Message);
        }

        #endregion

        #region �����Ų�ѯ�û����ԣ���ID��ȡ�û����ԣ���ѯȫ���Ų���
        /// <summary>
        /// ������ID��ѯ�û�����
        /// </summary>
        [Fact]
        public void GetUsersByDepartmentID_Default_Return()
        {
            var list = new List<User>() { new User { ID = 1, Name = "����ΰ", DepartmentID = 1, Password = "gsw", RoleID = 1, UserName = "gsw" } };
            _dbMock.Setup(db => db.Query<User>(It.IsAny<string>(), It.IsAny<object>(), null, true, null, null)).Returns(list);
            var users = _userRepository.GetUsersByDepartmentID(1);
            Assert.Single(list);
        }

        /// <summary>
        /// ��ID��ȡ�û�����
        /// </summary>
        [Fact]
        public void GetUser_Default_Return()
        {
            var list = new List<User>() { new User { ID = 1, Name = "����ΰ", DepartmentID = 1, Password = "gsw", RoleID = 1, UserName = "gsw" } };
            _dbMock.Setup(db => db.Query<User>(It.IsAny<string>(), It.IsAny<object>(), null, true, null, null)).Returns(list);
            var users = _userRepository.GetUser(1);
            Assert.Single(list);
        }
        /// <summary>
        /// ��ѯȫ���Ų���
        /// </summary>
        [Fact]
        public void GetDepartmentUsers_Default_Return()
        {
            var list = new List<UserRole>() {
                new UserRole{ ID=1, Name="����ΰ", DepartmentID=1, Password="gsw", RoleID=1, RoleName="Leader", UserName="gsw" }
            };
            _dbMock.Setup(db => db.Query<UserRole>(It.IsAny<string>(), It.IsAny<object>(), null, true, null, null)).Returns(list);
            var users = _userRepository.GetDepartmentUsers(1);
            Assert.Single(list);
        }
        #endregion


    }
}