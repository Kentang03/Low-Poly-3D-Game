# Main Menu UI Structure - Panduan Implementasi

## Tujuan
Mengatur struktur UI Main Menu agar ketika membuka panel (Settings, Credits, dll), hanya button-button utama yang disembunyikan sementara background tetap terlihat.

## Struktur UI Yang Diperlukan

```
MainMenuPanel (GameObject - berisi background dan semua elemen)
├── Background Image/Canvas (tetap terlihat)
├── Logo/Title (tetap terlihat)  
├── MainMenuButtonsGroup (GameObject - akan di hide/show)
│   ├── PlayButton
│   ├── SettingsButton
│   ├── CreditsButton
│   └── ExitButton
├── SettingsPanel (akan di show/hide)
├── CreditsPanel (akan di show/hide)
└── (elemen lain yang tetap terlihat)
```

## Cara Setup

### Opsi 1: Menggunakan Editor Tool (Recommended)
1. Buka menu `Tools > UI Setup > Main Menu Structure Setup`
2. Assign MainMenuPanel ke field yang tersedia
3. Klik "Auto Setup Structure"
4. Verifikasi setup di Inspector

### Opsi 2: Manual Setup
1. Buat Empty GameObject sebagai child dari MainMenuPanel
2. Namakan "MainMenuButtonsGroup"
3. Set RectTransform-nya to stretch full (anchor min: 0,0 - anchor max: 1,1)
4. Pindahkan semua button utama (Play, Settings, Credits, Exit) ke dalam group ini
5. Di MainMenuManager script, assign MainMenuButtonsGroup ke field `mainMenuButtonsGroup`

## Perubahan Pada MainMenuManager

Script telah dimodifikasi untuk:
- Menambah field `mainMenuButtonsGroup` untuk reference ke group button
- Mengubah logika show/hide dari panel menjadi button group
- Background (`mainMenuPanel`) tetap aktif sepanjang waktu
- Hanya `mainMenuButtonsGroup` yang di-toggle visibility-nya

## Keuntungan

1. **Background Konsisten**: Background, logo, dan elemen visual lain tetap terlihat
2. **Transisi Smooth**: Tidak ada flicker karena background tidak di-hide/show
3. **Modular**: Mudah menambah panel baru tanpa mengubah background
4. **Performance**: Lebih efisien karena tidak rebuild UI secara keseluruhan

## Testing

Setelah setup:
1. Test tombol Settings - background harus tetap terlihat
2. Test tombol Credits - background harus tetap terlihat  
3. Test tombol Back - button group harus muncul kembali
4. Pastikan semua transisi berjalan smooth

## Troubleshooting

**Button tidak tersembunyi**:
- Pastikan semua button utama berada dalam MainMenuButtonsGroup
- Cek assignment MainMenuButtonsGroup di MainMenuManager

**Background ikut tersembunyi**:
- Pastikan background/elemen visual berada langsung di MainMenuPanel, bukan dalam ButtonsGroup

**Layout rusak setelah setup**:
- Cek RectTransform MainMenuButtonsGroup (harus stretch full)
- Pastikan posisi button relatif terhadap ButtonsGroup, bukan MainMenuPanel