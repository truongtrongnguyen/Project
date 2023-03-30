﻿using BangHang.Models.OderProduct;
using System.Text;

namespace BangHang.Areas.Models.SendMail
{
    public static class SendMailAdmin
    {
        public static string OrderDetailList(List<OrderDetail> orderDetails)
        {
            var html = new StringBuilder();

            foreach (var item in orderDetails)
            {
                if (item.PriceSale > 0)
                {
                    html.Append(
                        $@"
                        <tr>
		                <td style=""color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;word-wrap:break-word"">
		                {item.NameProduct}</td>
		                <td style = ""color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif"" >
                            {item.Quantity} </td>

                        <td style = ""color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif"" >
                            <small style=""text-decoration: line-through; "">{item.PriceSale}</small>
                                        <br/>
                            <span> {item.Price} &nbsp;<span> VND </span></span></td>

                    </tr>
                    "
                        );
                }
                else
                {
                    html.Append(
                    $@"
                       <tr>
		                <td style=""color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;word-wrap:break-word"">
		                {item.NameProduct}</td>
		                <td style = ""color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif"" >
                            {item.Quantity} </td>

                        <td style = ""color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif"" >
                            <span> {item.Price} &nbsp;<span> VND </span></span></td>

                    </tr>
                    "
                    );
                }
            }
            return html.ToString();
        }
        public static string SendEMailAdmin(Order orderModel, List<OrderDetail> orderDetails)
        {

            return $@"

            <table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"">
                <tbody>
                    <tr>
                        <td align=""center"" valign=""top"">
                            <div>
                            </div>
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color:#ffffff;border:1px solid #dedede;border-radius:3px"">
                                <tbody>
                                    <tr>
                                        <td align=""center"" valign=""top"">

                                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color:#96588a;color:#ffffff;border-bottom:0;font-weight:bold;line-height:100%;vertical-align:middle;font-family:&quot;Helvetica Neue&quot;,Helvetica,Roboto,Arial,sans-serif;border-radius:3px 3px 0 0"">
                                                <tbody>
                                                    <tr>
                                                        <td style=""padding:36px 48px;display:block"">
                                                            <h1 style=""font-family:&quot;Helvetica Neue&quot;,Helvetica,Roboto,Arial,sans-serif;font-size:30px;font-weight:300;line-height:150%;margin:0;text-align:left;color:#ffffff;background-color:inherit"">Đơn hàng mới</h1>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td align=""center"" valign=""top"">

                                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                                                <tbody>
                                                    <tr>
                                                        <td valign=""top"" style=""background-color:#ffffff"">

                                                            <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                                <tbody>
                                                                    <tr>
                                                                        <td valign=""top"" style=""padding:48px 48px 32px"">
                                                                            <div style=""color:#636363;font-family:&quot;Helvetica Neue&quot;,Helvetica,Roboto,Arial,sans-serif;font-size:14px;line-height:150%;text-align:left"">

                                                                                <p style=""margin:0 0 16px"">Bạn vừa nhận được đơn hàng từ {orderModel.CustomerName}. Đơn hàng như sau:</p>
                                                                                <h2 style=""color:#96588a;display:block;font-family:&quot;Helvetica Neue&quot;,Helvetica,Roboto,Arial,sans-serif;font-size:18px;font-weight:bold;line-height:130%;margin:0 0 18px;text-align:left"">
                                                                                    <a href=""#"" style=""font-weight:normal;text-decoration:underline;color:#96588a"" target=""_blank"" data-saferedirecturl=""https://www.google.com/url?q=https://rarejuna.vn/wp-admin/post.php?post%3D4773%26action%3Dedit&amp;source=gmail&amp;ust=1656560185118000&amp;usg=AOvVaw2QhwJm1UcU7687gHkHSQ3N"">[Đơn hàng {orderModel.Code}]</a> ({orderModel.DateCreate})
                                                                                </h2>

                                                                                <div style=""margin-bottom:40px"">
                                                                                    <table cellspacing=""0"" cellpadding=""6"" border=""1"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;width:100%;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif"">
                                                                                        <thead>
                                                                                            <tr>
                                                                                                <th scope=""col"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Sản phẩm</th>
                                                                                                <th scope=""col"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Số lượng</th>
                                                                                                <th scope=""col"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Giá</th>
                                                                                            </tr>
                                                                                        </thead>
                                                                                        <tbody>
                                                                                            {OrderDetailList(orderDetails)}
                                                                                        </tbody>
                                                                                        <tfoot>
                                                                                            <tr>
                                                                                                <th scope=""row"" colspan=""2"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Phương thức thanh toán:</th>
                                                                                                <td style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">{orderModel.Payment}</td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <th scope=""row"" colspan=""2"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Tổng cộng:</th>
                                                                                                <td style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left""><span>{orderModel.TotalAmount}&nbsp;<span>VND</span></span></td>
                                                                                            </tr>
                                                                                        </tfoot>
                                                                                    </table>
                                                                                </div>


                                                                                <table cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width:100%;vertical-align:top;margin-bottom:40px;padding:0"">
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td valign=""top"" width=""50%"" style=""text-align:left;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;border:0;padding:0"">
                                                                                                <h2 style=""color:#96588a;display:block;font-family:&quot;Helvetica Neue&quot;,Helvetica,Roboto,Arial,sans-serif;font-size:18px;font-weight:bold;line-height:130%;margin:0 0 18px;text-align:left"">Thông tin người nhận</h2>

                                                                                                <address style=""padding:12px;color:#636363;border:1px solid #e5e5e5"">
                                                                                                    {orderModel.CustomerName}<br>
                                                                                                       Địa chỉ: {orderModel.Address}  
                                                                                                    <br><a href=""tel:{orderModel.Phone}"" style=""color:#96588a;font-weight:normal;text-decoration:underline"" target=""_blank"">{orderModel.Phone}</a>													<br><a href=""mailto:{orderModel.Email}"" target=""_blank"">{orderModel.Email}</a>
                                                                                                </address>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>

                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>

                </tbody>
            </table>

			";
        }

        public static string SendEMailClient(Order order, List<OrderDetail> orderDetails)
        {
            return $@"
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600""
	style=""background-color:#ffffff;border:1px solid #dedede;border-radius:3px"">
	<tbody>
		<tr>
			<td align=""center"" valign=""top"">

				<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%""
					style=""background-color:#96588a;color:#ffffff;border-bottom:0;font-weight:bold;line-height:100%;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;border-radius:3px 3px 0 0"">
					<tbody>
						<tr>
							<td style=""padding:36px 48px;display:block"">
								<h1
									style=""font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;font-size:30px;font-weight:300;line-height:150%;margin:0;text-align:left;color:#ffffff;background-color:inherit"">
									Cảm ơn bạn đã đặt hàng</h1>
							</td>
						</tr>
					</tbody>
				</table>

			</td>
		</tr>
		<tr>
			<td align=""center"" valign=""top"">

				<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
					<tbody>
						<tr>
							<td valign=""top"" style=""background-color:#ffffff"">

								<table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
									<tbody>
										<tr>
											<td valign=""top"" style=""padding:48px 48px 32px"">
												<div
													style=""color:#636363;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;font-size:14px;line-height:150%;text-align:left"">

													<p style=""margin:0 0 16px"">Xin chào {order.CustomerName},</p>
													<p style=""margin:0 0 16px"">Cảm ơn đã đặt hàng. Đơn hàng sẽ bị tạm
														giữ cho đến khi chúng tôi xác nhận thanh toán hoàn thành. Trong
														thời gian chờ đợi, đây là lời nhắc về những gì bạn đã đặt hàng:
													</p>


													<h2
														style=""color:#96588a;display:block;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;font-size:18px;font-weight:bold;line-height:130%;margin:0 0 18px;text-align:left"">
														[Đơn hàng {order.Code}] ({order.DateCreate})</h2>

													<div style=""margin-bottom:40px"">
														<table cellspacing=""0"" cellpadding=""6"" border=""1""
															style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;width:100%;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif"">
															<thead>
																<tr>
																	<th scope=""col""
																		style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">
																		Sản phẩm</th>
																	<th scope=""col""
																		style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">
																		Số lượng</th>
																	<th scope=""col""
																		style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">
																		Giá</th>
																</tr>
															</thead>
															<tbody>

																{OrderDetailList(orderDetails)}

															</tbody>
															<tfoot>
																<tr>
																	<th scope=""row"" colspan=""2""
																		style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">
																		Phương thức thanh toán:</th>
																	<td
																		style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">
																		{order.Payment}</td>
																</tr>
																<tr>
																	<th scope=""row"" colspan=""2""
																		style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">
																		Tổng cộng:</th>
																	<td
																		style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">
																		<span>{order.TotalAmount}&nbsp;<span>₫</span></span>
																	</td>
																</tr>
															</tfoot>
														</table>
													</div>


													<table cellspacing=""0"" cellpadding=""0"" border=""0""
														style=""width:100%;vertical-align:top;margin-bottom:40px;padding:0"">
														<tbody>
															<tr>
																<td valign=""top"" width=""50%""
																	style=""text-align:left;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;border:0;padding:0"">
																	<h2
																		style=""color:#96588a;display:block;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;font-size:18px;font-weight:bold;line-height:130%;margin:0 0 18px;text-align:left"">
																		Thông tin người nhận</h2>

																	<address
																		style=""padding:12px;color:#636363;border:1px solid #e5e5e5"">
																		{order.CustomerName}<br>
																		Địa chỉ: {order.Address}  
                                                                        <br><a href=""tel:{order.Phone}""
																			style=""color:#96588a;font-weight:normal;text-decoration:underline""
																			target=""_blank"">{order.Phone}</a> <br><a
																			href=""mailto:hungdd06@gmail.com""
																			target=""_blank"">{order.Email}</a>
																	</address>
																</td>
															</tr>
														</tbody>
													</table>
													<p style=""margin:0 0 16px"">Chúng tôi đang tiến hành hoàn thiện đơn
														đặt hàng của bạn</p>
												</div>
											</td>
										</tr>
									</tbody>
								</table>

							</td>
						</tr>
					</tbody>
				</table>

			</td>
		</tr>
	</tbody>
</table>
            ";
        }
    }
}
