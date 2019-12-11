// <copyright file="RpcExceptionUtil.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;
using Grpc.Core;

namespace GeothermalResearchInstitute.Wpf.Common
{
    internal static class RpcExceptionUtil
    {
        public static void ShowMessageBox(this RpcException e)
        {
            string text = e.StatusCode switch
            {
                StatusCode.DeadlineExceeded | StatusCode.Unavailable => "网络连接错误，请检查到服务器的连接是否正常",
                StatusCode.Unauthenticated => "用户名或密码错误",
                _ => "其他未知错误：\n" + e.ToString(),
            };

            MessageBox.Show(text, "远程连接错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
