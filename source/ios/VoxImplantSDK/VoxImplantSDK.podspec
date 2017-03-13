Pod::Spec.new do |s|
  s.name         = "VoxImplantSDK"
  s.version      = "2.0.0"
  s.summary      = "VoxImplant SDK for iOS"
  s.description  =  <<-DESC
    VoxImplant SDK for iOS. Supports armv7, arm64, x86_64 architectures.
                   DESC

  s.homepage     = "http://voximplant.com"
  s.license      = { :type => "Commercial", :file => "LICENSE.txt" }
  s.authors      = "Zingaya, Inc."

  s.platform     = :ios, "8.0"
  s.source       = { :path => '.' }
  

  s.public_header_files = "VoxImplant.framework/Headers/**/*.h"
  s.source_files = "VoxImplant.framework/Headers/**/*.h"

  s.vendored_frameworks = "VoxImplant.framework"
  s.dependency "VoxImplantWebRTC", '~> 57.0.0'

end
