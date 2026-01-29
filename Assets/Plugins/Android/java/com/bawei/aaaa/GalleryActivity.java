package com.bawei.aaaa;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.content.ContentResolver;
import java.io.InputStream;
import java.io.FileOutputStream;
import java.io.File;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class GalleryActivity extends UnityPlayerActivity {
    
    private static final int PICK_IMAGE_REQUEST = 1001;
    private static final String TAG = "GalleryActivity";
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "GalleryActivity onCreate");
    }
    
    // Method called from Unity
    public void openGallery() {
        Log.d(TAG, "openGallery called from Unity");
        try {
            Intent intent = new Intent(Intent.ACTION_PICK);
            intent.setType("image/*");
            Log.d(TAG, "Starting activity for result with ACTION_PICK");
            startActivityForResult(intent, PICK_IMAGE_REQUEST);
        } catch (Exception e) {
            Log.e(TAG, "Failed to open gallery", e);
            UnityPlayer.UnitySendMessage("GalleryManager", "OnGalleryError", "Failed to open gallery: " + e.getMessage());
        }
    }
    
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        Log.d(TAG, "onActivityResult: requestCode=" + requestCode + ", resultCode=" + resultCode);
        
        if (requestCode == PICK_IMAGE_REQUEST) {
            if (resultCode == RESULT_OK) {
                Log.d(TAG, "RESULT_OK received");
                if (data != null) {
                    Uri uri = data.getData();
                    if (uri != null) {
                        String uriString = uri.toString();
                        Log.d(TAG, "Selected image URI: " + uriString);
                        
                        // 尝试将文件复制到缓存目录并返回文件路径
                        String filePath = copyImageToCache(uri);
                        if (filePath != null) {
                            Log.d(TAG, "File copied to cache: " + filePath);
                            UnityPlayer.UnitySendMessage("GalleryManager", "OnImageSelected", filePath);
                        } else {
                            // 如果复制失败，返回原始URI
                            UnityPlayer.UnitySendMessage("GalleryManager", "OnImageSelected", uriString);
                        }
                    } else {
                        Log.w(TAG, "URI is null");
                        UnityPlayer.UnitySendMessage("GalleryManager", "OnGalleryError", "Failed to get image URI");
                    }
                } else {
                    Log.w(TAG, "Data is null");
                    UnityPlayer.UnitySendMessage("GalleryManager", "OnGalleryError", "No data received");
                }
            } else {
                Log.d(TAG, "User cancelled or error, resultCode: " + resultCode);
                UnityPlayer.UnitySendMessage("GalleryManager", "OnGalleryError", "User cancelled selection");
            }
        } else {
            Log.d(TAG, "Ignoring non-PICK_IMAGE_REQUEST result");
        }
    }
    
    /**
     * 将图片复制到应用缓存目录
     */
    private String copyImageToCache(Uri uri) {
        InputStream inputStream = null;
        FileOutputStream outputStream = null;
        
        try {
            // 获取ContentResolver
            ContentResolver contentResolver = getContentResolver();
            
            // 打开输入流
            inputStream = contentResolver.openInputStream(uri);
            if (inputStream == null) {
                Log.e(TAG, "Cannot open input stream");
                return null;
            }
            
            // 创建缓存文件
            File cacheDir = getCacheDir();
            String fileName = "selected_image_" + System.currentTimeMillis() + ".jpg";
            File outputFile = new File(cacheDir, fileName);
            
            // 创建输出流
            outputStream = new FileOutputStream(outputFile);
            
            // 复制数据
            byte[] buffer = new byte[8192];
            int bytesRead;
            int totalBytes = 0;
            
            while ((bytesRead = inputStream.read(buffer)) != -1) {
                outputStream.write(buffer, 0, bytesRead);
                totalBytes += bytesRead;
            }
            
            Log.d(TAG, "Image copied successfully: " + totalBytes + " bytes");
            
            // 返回文件路径
            return outputFile.getAbsolutePath();
            
        } catch (Exception e) {
            Log.e(TAG, "Failed to copy image to cache", e);
            return null;
        } finally {
            try {
                if (inputStream != null) {
                    inputStream.close();
                }
                if (outputStream != null) {
                    outputStream.close();
                }
            } catch (Exception e) {
                Log.e(TAG, "Error closing streams", e);
            }
        }
    }
}