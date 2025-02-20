
## 背景
Xinference的启动方式太TM不优雅了，想要使用一个类似Ollama的方式启动Xinference，同时该方法也适用于其他需要Web服务器或前台CMD启动的程序 

The startup method of Xinference is not elegant enough. I want to use a similar way to Olama to start Xinference, and this method is also applicable to other programs that require web servers or front-end CMD startup

## 功能
1.后台启动Xinference并注册为服务在后台运行Xinference

![image](https://github.com/user-attachments/assets/ceab6409-0e17-49ef-8cd3-e5ad4b3c901a)


2.显示运行状态

*服务暂停中*

![image](https://github.com/user-attachments/assets/d8a5468f-4ca7-4180-b7e8-47f72156f5fc)

*服务启动中*

![image](https://github.com/user-attachments/assets/9f401211-6e2e-4609-a73d-533437cb4877)

3.通过右键菜单控制服务暂停、启动和退出

![image](https://github.com/user-attachments/assets/1c32f0e0-bbac-4d72-b007-0a9ee978766c)


## 使用方法：
### 通过conda正常安装Xinference
Xinference过程我就不说了，说白了就是使用conda安装Xinference

### 创建一个系统批处理文件(.bat)
内容为：
```
@echo off

call activate xin

xinference-local

pause
```
复制到自己的.bat文件中

> *注意call activate xin中的xin替换为你自己的conda环境名称*
> 在cmd中使用以下命令查看名称和位置
> ```
> conda env list
> ```

### 使用nssm将创建的.bat文件注册为系统服务在后台运行
这一步看教程
教程链接：
https://www.bilibili.com/video/BV13b4y1j7xx/?spm_id_from=333.337.search-card.all.click&vd_source=55e8e7375185ec175c900fd19c2ed6d2

>
>但是！nssm创建时后台程序名称 一定!一定！要是：
>```
>AI.xinference
>```


### 打开程序
到目前其实Xinference已经可以自启动并优雅运行了，但是还是不够优雅！

现在打开程序就可以控制Xinference的启动暂停和退出了

### 关于更改程序与售后
欢迎关注我的B站账号

![image](https://github.com/user-attachments/assets/48b0549c-d29f-441c-82af-593ba3fc38a1)

我的B站账号：https://space.bilibili.com/18732435?spm_id_from=333.1007.0.0

