package com.voximplant.sdk.permissions;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.RequiresApi;
import android.util.Log;
import android.view.View;

public class PermissionsActivity extends Activity {
    public static final String PERMISSION_EXTRA = "permissions.PermissionArray";
    private static final int RC_ASK_PERMISSION = 778;
    private static final String TAG = "PermissionsActivity";
    private boolean allPermissionsGranted;
    private String[] permissionArray;

    public PermissionsActivity() {
        this.allPermissionsGranted = false;
    }

    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        View view = new View(this);
        view.setBackgroundColor(Color.BLACK);
        setContentView(view);

        Intent myIntent = getIntent();
        this.permissionArray = null;
        if (myIntent.hasExtra(PERMISSION_EXTRA)) {
            this.permissionArray = myIntent.getStringArrayExtra(PERMISSION_EXTRA);
        }
        if (this.permissionArray == null || this.permissionArray.length == 0) {
            Log.w(TAG, "No permissions requested!");
            PermissionsFragment.setPermissionResult(true);
            finish();
        }
    }

    @RequiresApi(api = Build.VERSION_CODES.M)
    @Override
    protected void onResume() {
        super.onResume();

        requestPermissions(this.permissionArray, RC_ASK_PERMISSION);
    }

    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        if (requestCode == RC_ASK_PERMISSION) {
            this.allPermissionsGranted = true;
            for (int result : grantResults) {
                if (result != 0) {
                    this.allPermissionsGranted = false;
                    break;
                }
            }

            PermissionsFragment.setPermissionResult(allPermissionsGranted);
            finish();
        } else {
            super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}