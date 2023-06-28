# mo_hinh_n_layer

# tạo database từ đầu
- xóa các file gen trong folder migration. Chọn defualt project Data, rồi chạy command bên dưới.

  
EntityFrameworkCore\add-migration initDbContext -context BookingOnlineDbContext -StartupProject IdentityServer
EntityFrameworkCore\add-migration initIdentity -context PersistedGrantDbContext -StartupProject IdentityServer
EntityFrameworkCore\add-migration initConfig -context ConfigurationDbContext -StartupProject IdentityServer

EntityFrameworkCore\Update-Database -context BookingOnlineDbContext -StartupProject IdentityServer
EntityFrameworkCore\Update-Database -context PersistedGrantDbContext -StartupProject IdentityServer
EntityFrameworkCore\Update-Database -context ConfigurationDbContext -StartupProject IdentityServer

- command gen scitp sql file
EntityFrameworkCore\Script-Migration -From 20230424013447_20230424 -o "App.BookingOnline.Data/Migrations/script/20230626.sql" -context BookingOnlineDbContext -StartupProject IdentityServer
