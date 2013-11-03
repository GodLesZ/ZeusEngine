namespace Zeus.Library.Models {

    public interface IAccount : IModel {

        long Id { get; }

        string Login { get; }

        string Password { get; }

    }

}