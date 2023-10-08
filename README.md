# SoftwareAuthorizationVerification

# 软件授权校验

## 特性：

1. RSA 非对称加密，公钥加密，私钥解密，保证license信息安全
1. Digital Signature公钥数字签名:
	1. 防止license被冒充签发（认证）；
	2. 保证数据完整性；
	3. 数字签名具有不可抵赖性（即不可否认性）
1. 硬件信息采集，防止程序被无限copy
1. 授权截止时间，完成业务上授权需求
1. 使用license的业务代码混淆加密，防止反编译替换跳过验证流程
1. 可以加入自定义数据（授权版本、授权对象、授权功能列表）等等，方便扩展

### 如何在程序上使用授权验证

1. 项目引用SAVC.dll文件
2. 将生产的license.lic 文件拷贝至运行项目根目录下
3. 在项目关键Controller或程序启动函数中加上授权文件相关代码

```csharp
    //获取lic文件，默认获取根目录下的license.lic文件
    License license = License.GetLicense()
    //获取主版本，副版本，产品类型，序列号，过期日期，用户信息（如权限集合），签名 等信息
    license.Copyright
    license.LicenceTo
    license.ProductName
    license.MajorVersion
    license.MinorVersion
    license.MachineHash
    license.ExpireTo
    license.license.UserData
    license.DaysLeftInTrial
    license.SerialNumber
```

​    4.验证、使用

```csharp
	//验证日期、验证MachineHash
 	bool License.VerifyLicense(License lic);

 	//也可以自己利用参数值做判断
	if(license.ExpireTo > Date.now()){
		//过期
	}

	if(license.DaysLeftInTrial < 30){
		//即将过期，小于30天。可以提示用户
	}

	//利用UserData，将系统的菜单权限带过来
	//license.UserData = 01,0101,0102......
	String[] menus = license.UserData.split(",");
```

## 贡献

有任何意见或建议都欢迎提 issue



## 其他说明 

### 异常处理

在操作数据库时，可能会抛出数据库相关的异常，用户应该根据需要进行适当的异常处理。



### 许可证信息

这个工具类遵循 [MIT 许可证](https://opensource.org/licenses/MIT)。



### 联系方式

如果您有任何问题或需要支持，请联系我：[wei.zhou@ccssttcn.com](mailto:wei.zhou@ccssttcn.com)
