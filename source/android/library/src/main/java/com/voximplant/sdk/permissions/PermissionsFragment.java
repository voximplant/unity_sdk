package com.voximplant.sdk.permissions;

import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentTransaction;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.support.annotation.RequiresApi;
import android.util.Log;

public class PermissionsFragment extends Fragment {
    public static final String FRAGMENT_TAG = "vox_PermissionsFragment";
    private static final String TAG = "PermissionsFragment";
    private static PermissionsCallback permissionsCallback = null;

    public interface PermissionsCallback {
        void onRequestPermissionResult(boolean z);
    }

    public static boolean RequirePermissionsRequests() {
        return Build.VERSION.SDK_INT >= 23;
    }

    public static PermissionsFragment getInstance(Activity parentActivity) {
        Throwable th;
        PermissionsFragment fragment = (PermissionsFragment) parentActivity.getFragmentManager().findFragmentByTag(FRAGMENT_TAG);
        if (fragment == null) {
            try {
                Log.i(TAG, "Creating PermissionsFragment");
                PermissionsFragment fragment2 = new PermissionsFragment();
                try {
                    FragmentTransaction trans = parentActivity.getFragmentManager().beginTransaction();
                    trans.add(fragment2, FRAGMENT_TAG);
                    trans.commit();
                    fragment = fragment2;
                } catch (Throwable th2) {
                    th = th2;
                    fragment = fragment2;
                    Log.e(TAG, "Cannot launch PermissionsFragment:" + th.getMessage(), th);
                    return null;
                }
            } catch (Throwable th3) {
                th = th3;
                Log.e(TAG, "Cannot launch PermissionsFragment:" + th.getMessage(), th);
                return null;
            }
        }
        return fragment;
    }

    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setRetainInstance(true);
    }

    @RequiresApi(api = Build.VERSION_CODES.M)
    public boolean hasPermission(String permission) {
        Log.d(TAG, "Checking permission " + permission);
        return getActivity().checkSelfPermission(permission) == PackageManager.PERMISSION_GRANTED;
    }

    @RequiresApi(api = Build.VERSION_CODES.M)
    public boolean[] hasPermissions(String[] permissions) {
        if (permissions == null) {
            Log.w(TAG, "No permission asked, no permissions returned");
            return new boolean[0];
        }
        int length = permissions.length;
        boolean[] grantResults = new boolean[length];
        for (int i = 0; i < length; i++) {
            Log.d(TAG, "Checking permission for " + permissions[i]);
            grantResults[i] = hasPermission(permissions[i]);
        }
        return grantResults;
    }

    public boolean shouldShowRational(String permission) {
        return Build.VERSION.SDK_INT >= 23 && getActivity().shouldShowRequestPermissionRationale(permission);
    }

    public void requestPermission(final String[] permissionArray, PermissionsCallback callback) {
        permissionsCallback = callback;
        getActivity().runOnUiThread(new Runnable() {
            @RequiresApi(api = Build.VERSION_CODES.M)
            @Override
            public void run() {
                Intent intent = new Intent(PermissionsFragment.this.getContext(), PermissionsActivity.class);
                intent.putExtra(PermissionsActivity.PERMISSION_EXTRA, permissionArray);
                PermissionsFragment.this.startActivity(intent);
            }
        });
    }

    public static void setPermissionResult(boolean allGranted) {
        if (permissionsCallback != null) {
            permissionsCallback.onRequestPermissionResult(allGranted);
        } else {
            Log.w(TAG, "Permission callback object is null!");
        }
    }
}