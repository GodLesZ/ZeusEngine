namespace Zeus.Library.Models {

    public interface IAccount : IModel {

        int Id { get; }

        string Login { get; }

        string Password { get; }

    }

}