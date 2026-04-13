[SecurityCritical]
[SecurityTreatAsSafe]
protected override Stream GetStreamCore(FileMode mode, FileAccess access)
{
    Stream stream = null;
    stream = EnsureResourceLocationSet();
    if (stream == null)
    {
        stream = _rmWrapper.Value.GetStream(_fullname);
        if (stream == null)
        {
            throw new IOException(SR.Get("UnableToLocateResource", _fullname));
        }
    }
    ContentType contentType = new ContentType(base.ContentType);
    if (MimeTypeMapper.BamlMime.AreTypeAndSubTypeEqual(contentType))
    {
        BamlStream bamlStream = new BamlStream(stream, _rmWrapper.Value.Assembly);
        stream = bamlStream;
    }
    return stream;
}