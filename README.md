<div align="center">

<img src="icon96.png" width="96" alt="Always-on-Top icon" />

# Always-on-Top · 視窗透明器

**替任意 Windows 視窗調整透明度、滑鼠穿透、置頂與位置的輕量小工具**

單一 `.exe`・約 90 KB・免安裝・Windows 10 / 11 不需要額外執行環境

</div>

---

## 這是什麼

掛機遊戲、看影片、盯著圖表或聊天視窗時，常常希望某個視窗「看得到，但不要擋住下面的操作」。
Always-on-Top 讓你選定任何一個視窗，然後：

- 把它變透明（0～100%）
- 讓滑鼠直接穿透它，操作下方的程式
- 讓它保持在最上層
- 一鍵貼齊螢幕的角落或置中

## 功能

| 功能 | 說明 |
| --- | --- |
| 選取視窗 | 從清單點選（附程式圖示），或用「拖曳準星」放開在目標視窗上 |
| 透明度 | 0～100% 滑桿，另有 0 / 25 / 50 / 75 / 100% 快速按鈕 |
| 鎖定互動 | 視窗仍然可見，但滑鼠操作會穿透它，交給下方的視窗；再切換一次即可解鎖 |
| 保持置頂 | 讓目標視窗維持在其他視窗之上 |
| 視窗位置 | 左上、右上、置中、左下、右下，依視窗當下大小貼齊螢幕工作區（不含工作列） |
| 還原 | 可還原選定視窗，或一鍵全部還原；結束程式時也會自動還原 |
| 全域快捷鍵 | 鎖定後點不到視窗也沒關係，用快捷鍵就能操作 |

另外：深色介面、單一實例（重複開啟不會衝突）、縮小後留在工作列。

## 下載與使用

1. [下載 Always-on-Top.exe](https://github.com/zihuanfeng520/Always-on-Top/releases/download/v1.0.0/Always-on-Top.exe)（也可以到 [Releases](https://github.com/zihuanfeng520/Always-on-Top/releases) 頁面查看所有版本）。
2. 雙擊執行，不需要安裝。
3. 在清單選擇目標視窗，或按住「拖曳準星到目標視窗」，拖到目標視窗上放開。
4. 調整透明度、開關鎖定互動與置頂，或點「視窗位置」的按鈕。

> 若 Windows SmartScreen 擋下（因為檔案沒有數位簽章），點「其他資訊」→「仍要執行」即可。
> 想確認安全性，可以直接閱讀 [`Always-on-Top.cs`](Always-on-Top.cs)，並自行編譯。

## 快捷鍵

全部都是 `Ctrl + Alt +` 搭配下列按鍵：

| 按鍵 | 功能 |
| --- | --- |
| `L` | 鎖定／解鎖（滑鼠穿透）選定的視窗 |
| `↑` / `↓` | 透明度 +10% / −10% |
| `H` | 呼出面板（面板已在前景時則縮小） |
| `R` | 緊急還原：一鍵還原所有被改過的視窗 |

面板開啟時按 `Esc` 會縮小到工作列。

## 面板的行為

- 面板會壓在所有置頂視窗之上，所以即使目標視窗被鎖定並置頂，仍然點得到面板。
- 按「—」或「✕」都是縮小到工作列；右上角的「⏻」或 `Alt + F4` 才會結束程式。
- 結束程式時，會自動把所有被改過的視窗還原成原本的狀態。

## 系統需求

- Windows 10 / 11（已內建所需的 .NET Framework 4.x）
- Windows 7 / 8 需自行安裝 .NET Framework 4.x，且尚未經過測試

## 自行編譯

不需要安裝 Visual Studio，使用 Windows 內建的 C# 編譯器即可。
將 `Always-on-Top.cs`、`icon.ico`、`icon96.png`、`build.bat` 放在同一個資料夾，執行：

```bat
build.bat
```

或手動執行：

```bat
%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe ^
  /target:winexe /optimize+ /win32icon:icon.ico ^
  /resource:icon96.png,icon96.png ^
  /reference:System.Windows.Forms.dll /reference:System.Drawing.dll ^
  /out:Always-on-Top.exe Always-on-Top.cs
```

## 運作原理

Always-on-Top 只使用 Windows 公開的視窗 API，不注入程式碼，也不讀寫其他程式的記憶體：

- 透明度：為目標視窗加上 `WS_EX_LAYERED` 並呼叫 `SetLayeredWindowAttributes`
- 滑鼠穿透：加上 `WS_EX_TRANSPARENT` 擴充樣式
- 置頂與位置：`SetWindowPos`，貼齊時以 DWM 的實際可見邊界計算，排除視窗周圍看不見的陰影
- 快捷鍵：`RegisterHotKey`

## 已知限制

- **以系統管理員身分執行的程式**：Always-on-Top 也要以系統管理員身分執行，才控制得到它們。
- **部分程式不吃透明度**：少數使用特殊繪圖方式的程式（例如部分 UWP 或瀏覽器的特殊模式）可能無法套用透明度。
- **全螢幕獨占模式**：這類遊戲會直接接管顯示，面板與透明效果都可能無法顯示，建議改用「無邊框視窗」或「視窗化」。
- **貼齊為單次動作**：點一下就依當下大小移動一次，之後再縮放視窗不會自動重新貼齊。
- **多螢幕**：貼齊的是目標視窗目前所在的那個螢幕的工作區。

## 關於線上遊戲與反作弊

本工具只改變視窗的外觀與輸入穿透，沒有任何針對遊戲內容的功能。
但部分反作弊系統（例如 Vanguard、EasyAntiCheat、BattlEye）可能會偵測「修改遊戲視窗屬性」或不明的覆蓋程式，**有誤判的可能，作者無法保證**。
另外，使用工具進行自動掛機本身可能違反某些遊戲的服務條款。請在使用前自行確認該遊戲的規定，風險由使用者自行承擔。

## 專案結構

編譯好的 `Always-on-Top.exe` 放在 [Releases](https://github.com/zihuanfeng520/Always-on-Top/releases)，不在 repo 內。

```text
.
├── Always-on-Top.cs   # 完整原始碼（單一檔案）
├── icon.ico           # 程式圖示（內嵌於 exe）
├── icon96.png         # 介面標題用的圖示
├── build.bat          # 一鍵編譯腳本
└── LICENSE            # MIT 授權
```

## 授權

原始碼以 [MIT License](LICENSE) 釋出，可自由使用、修改、散布與商用，只需保留版權與授權聲明。

程式圖示為本專案原創（兩個視窗與圖釘），同樣以 MIT 授權釋出。
