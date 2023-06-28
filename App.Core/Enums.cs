using System.ComponentModel.DataAnnotations;

namespace App.Core
{
    public class Enums
    {
        public enum PromotionType
        {
            normal,
            seagolf
        }
        public enum BookingButtonType
        {
            TeesheetStep,
            PersonInfoStep,
            PriceCheckStep,
            Payment,
            BookingEnd
        }

        public enum TeesheetType
        {
            morning = 1,
            afternoon = 2,
            evening = 3
        }
        public enum StatusBookingLine
        {
            [Display(Name = "Đặt vé")]
            open, // teetime thanh cong

            [Display(Name = "Chọn người chơi")]
            bookedTeetime, // chon nguoi choi thanh cong

            [Display(Name = "Thanh toán")]
            priced, // tinh gia thanh cong

            [Display(Name = "Đặt vé thành công")]
            booked, // thanh toan thanh cong

            [Display(Name = "Thanh toán không thành công")]
            close,  // thanh toan khong thanh cong

            [Display(Name = "Hủy chơi")]
            cancel // huy choi golf
        }

        public enum CancelStatus
        {
            notRefund = 1, // huy booking khong hoan tien
            cancelRefund50 = 21, // huy booking hoan tien
            cancelRefund100 = 22, // huy booking hoan tien    
            manualCancel = 3, // huy booking o man hinh xac nhan   
        }
        public enum StatusTransLine
        {
            Map = 1, // map với file out
            NotMap = 2, // map với file in
            MoneyGoAcc = 3, // xác nhận tiền về tài khoản
            CancelLine = 4, // trạng thái hủy ở màn hình xác nhận tiền về
            RefundMoney = 5,// hủy hoàn tiền
            MoneyOutAcc = 6, // xác nhận tiền ra ngoài tài khoản
            MoneyOutIn = 7 // trạng thai xác nhận tiền vừa về vừa ra khỏi tài khoản, trường hợp hủy
        }


        public enum MemberCardType
        {
            [Display(Name = "Member")]
            member = 1,
            [Display(Name = "Member Brg")]
            memberBrg = 22,
            visitor = 5,
            [Display(Name = "Member Guest")]
            memberGuest = 2
        }

        public enum MemberCardRequestStatus
        {
            open = 1,
            reject = 2,
            approve = 3,
            close = 4
        }

        public enum ValidNumPlayerBook
        {
            NotCurrentUser = 2,
            CurrentUser = 4
        }
        public enum DeviceCode
        {
            mobile,
            web
        }
        public enum FcmNotifiType
        {
            booking = 1,
            promotion = 2,
            seagolf = 3,
            notifiAll = 4,
            maintain = 5,
            SendEmailBookingCourse = 6
        }

        public enum PromotionFormula
        {
            percent,
            price
        }

        public enum AccountStatus
        {
            open = 0,
            active = 1,
            inActive = 2,
            locked = 3,
            moreNoShow = 4,
            noLockNomore = 100 // never log by not show up on course
        }
        public enum CancelBookingType
        {
            PC = 1 // player cancel a book
        }
        public enum RoleEnums
        {
            customer,
            admin,
            employee
        }

        public enum TransBookStatus
        {
            created = 1,
            inApprove = 2,
            outApprove = 3,
            delted = 4
        }

        public enum LogDateFileOutStatus
        {
            created = 1,
            sent = 2,
            deleted = 3
        }

        public enum FtpStatus
        {
            download,
            upload
        }

        public enum TotalAllowVoucher
        {
            Kigr_voucher = 400,
            Lhgr_voucher = 200,
            Rtgr_voucher = 200,
            Dngr_voucher = 200
        }

        public enum FileType
        {
            In = 1,
            Out = 2
        }
    }
}
